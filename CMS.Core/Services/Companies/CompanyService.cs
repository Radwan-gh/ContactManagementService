using Ardalis.GuardClauses;
using CMS.Core.Common;
using CMS.Core.Companys.Dtos;
using CMS.Core.Constants;
using CMS.Core.Contacts.Dtos;
using CMS.Core.Entities;
using CMS.Core.Enums;
using CMS.Core.Extentions;
using CMS.Core.Helpers;
using CMS.Core.Interfaces;
using CMS.Core.Services.IdentifierSequences;
using CMS.SharedKernel;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Core.Companies.Services
{
    public class CompanyService : ICompanyService
    {
        private readonly IMongodbContext _mongodbContext;
        private readonly IIdentifierSequenceService _identifierSequenceService;
        public CompanyService(IMongodbContext mongodbContext, IIdentifierSequenceService identifierSequenceService)
        {
            _mongodbContext = mongodbContext;
            _identifierSequenceService = identifierSequenceService;
        }

        public async Task<ServiceResult<CompanyDetailsDTO>> GetById(int id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NegativeOrZero(id, nameof(id));

            if ((await _mongodbContext.Companies.FindAsync(c => c.Id == id))
                .FirstOrDefault() is Company company)
                return new(CompanyDetailsDTO.FromEntity(company));

            return new(default, false, "company not found!");
        }

        public async Task<PaginatedResponse<SelectItem>> GetAll(int page, int pageSize)
        {
            var (total, data) = await _mongodbContext.Companies.AggregateByPage(
                Builders<Company>.Filter.Empty,
                Builders<Company>.Sort.Ascending(c => c.Id),
                 (page - 1) * pageSize, pageSize
                );
            var companies = data.Select(c => new SelectItem(c.Id,
                   c.CustomAttributes.FirstOrDefault(ca => ca.Name == FixedAttributes.Name).Value));

            return new PaginatedResponse<SelectItem>(companies, total, page, pageSize);
        }

        public async Task<ServiceResult> Create(CompanyDTO companyDTO)
        {
            var res = await PrepareEntity(companyDTO);

            if (!res.IsSuccess)
                return res;

            var company = res.Data;
            company.Id = _identifierSequenceService.GetIdentifier(EntityType.Company);

            await _mongodbContext.Companies.InsertOneAsync(company);
            return new();
        }

        public async Task<ServiceResult> Update(CompanyDTO companyDto)
        {
            Guard.Against.NegativeOrZero(companyDto.Id, nameof(companyDto.Id));

            var res = await PrepareEntity(companyDto);

            if (!res.IsSuccess)
                return new ServiceResult<ServiceResult>(res);

            var replaceResult = (await _mongodbContext.Companies.ReplaceOneAsync(
                Builders<Company>.Filter.Eq(c => c.Id, companyDto.Id),
                res.Data)).IsAcknowledged;

            // update companies in contacts
            _mongodbContext.Contacts.UpdateMany(
                c => c.Companies.Any(com => com.Id == companyDto.Id),
                Builders<Contact>.Update.Set(x => x.Companies[-1], res.Data));

            if (!replaceResult)
                return new(false, "update faild!");

            return new();
        }

        public async Task<ServiceResult> DeleteCompany(int id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NegativeOrZero(id, nameof(id));

            if (!(await _mongodbContext.Companies.DeleteOneAsync(c => c.Id == id)).IsAcknowledged)
                return new(false);

            // update companies in contacts
            _mongodbContext.Contacts.UpdateMany(
               Builders<Contact>.Filter.Empty,
                Builders<Contact>.Update.PullFilter(x => x.Companies, c => c.Id == id));

            return new();
        }

        private async Task<bool> CheckUniqeName(CompanyDTO companyDTO, int id = 0)
        {
            // validate name unique constraint
            var filter = Builders<Company>.Filter;
            var filterDefinition = filter.ElemMatch(x => x.CustomAttributes,
                att => att.Name == FixedAttributes.Name
                && att.Value == companyDTO.Name);

            if (id != 0)
                filterDefinition &= filter.Ne(c => c.Id, id);

            if ((await _mongodbContext.Companies.CountDocumentsAsync(filterDefinition)) > 0)
                return false;

            return true;
        }

        private async Task<ServiceResult<Company>> PrepareEntity(CompanyDTO companyDTO)
        {
            Guard.Against.Null(companyDTO, nameof(companyDTO));

            if (!await CheckUniqeName(companyDTO, companyDTO.Id))
                return new ServiceResult<Company>(default, false, "the company name already exists!");

            var company = new Company();

            if (companyDTO.Id != 0)
                company.Id = companyDTO.Id;

            company.CustomAttributes = companyDTO.CustomAttributes.ToList();

            company.CustomAttributes.Add(new CustomAttributeRecord(FixedAttributes.Name,
              companyDTO.Name, (int)AttributeType.String));

            // Validate values types'
            foreach (var attribute in companyDTO.CustomAttributes)
            {
                switch (attribute.Type)
                {
                    case AttributeType.Int:
                        if (!int.TryParse(attribute.Value, out int intVal))
                            return new ServiceResult<Company>(default, false, $"{attribute.Name} value not valid, must be number!");
                        break;
                    case AttributeType.DateTime:
                        if (!attribute.Value.ValidateDate())
                            return new ServiceResult<Company>(default, false,
                                $"{attribute.Name} value not valid, must be datetime in this format {DateTimeExtention._DateFormat}!");
                        break;
                }
            }

            return new ServiceResult<Company>(company);
        }
    }
}
