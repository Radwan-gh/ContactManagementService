using Ardalis.GuardClauses;
using CMS.Core.Constants;
using CMS.Core.Contacts.Dtos;
using CMS.Core.Entities;
using CMS.Core.Enums;
using CMS.Core.Extentions;
using CMS.Core.Helpers;
using CMS.Core.Interfaces;
using CMS.Core.Services.IdentifierSequences;
using CMS.Infrastructure.Data;
using CMS.SharedKernel;
using MongoDB.Bson;
using MongoDB.Driver;
using System.Linq;
using System.Threading.Tasks;
namespace CMS.Core.Contacts.Services
{
    public class ContactService : IContactService
    {
        private readonly IMongodbContext _mongodbContext;
        private readonly IIdentifierSequenceService _identifierSequenceService;

        public ContactService(IMongodbContext mongodbContext, IIdentifierSequenceService identifierSequenceService)
        {
            _mongodbContext = mongodbContext;
            _identifierSequenceService = identifierSequenceService;
        }
        public async Task<ServiceResult<ContactDetailsDTO>> GetContactById(int id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NegativeOrZero(id, nameof(id));

            if ((await _mongodbContext.Contacts.FindAsync(c => c.Id == id))
                .FirstOrDefault() is Contact contact)
                return new(ContactDetailsDTO.FromEntity(contact));

            return new(default, false, "contact not found!");
        }
        public async Task<ServiceResult> CreateContact(ContactDTO contactDto)
        {
            var res = await PrepareEntity(contactDto);

            if (!res.IsSuccess)
                return res;

            var contact = res.Data;
            contact.Id = _identifierSequenceService.GetIdentifier(EntityType.Contact);

            await _mongodbContext.Contacts.InsertOneAsync(contact);
            return new();
        }
        public async Task<ServiceResult> UpdateContact(ContactDTO contactDto)
        {
            Guard.Against.NegativeOrZero(contactDto.Id, nameof(contactDto.Id));

            var res = await PrepareEntity(contactDto);

            if (!res.IsSuccess)
                return res;

            var replaceResult = (await _mongodbContext.Contacts.ReplaceOneAsync(
                Builders<Contact>.Filter.Eq(c => c.Id, contactDto.Id),
                res.Data)).IsAcknowledged;

            if (!replaceResult)
                return new(false, "update faild!");

            return new();
        }
        public async Task<ServiceResult<PaginatedResponse<ContactDTO>>> Search(ContactSearchDto dto)
        {
            // TODO : validate types

            // search 
            var filter = Builders<Contact>.Filter;
            var filterDefinitio = FilterDefinition<Contact>.Empty;

            foreach (var search in dto.SearchAttributes)
            {
                var filterPath = search.EntityType == EntityType.Contact ? "CustomAttributes" : "Companies.CustomAttributes";
                filterDefinitio &= filter.Eq($"{filterPath}.Name", search.AttributeName);

                switch (search.FilterType)
                {
                    case FilterType.Contains:
                        filterDefinitio &= filter.Regex($"{filterPath}.Value", new BsonRegularExpression(search.SearchTerm, "i"));
                        break;

                    case FilterType.LessThan:
                        filterDefinitio &= filter.Lt($"{filterPath}.Value", search.SearchTerm);
                        break;

                    case FilterType.GreaterThan:
                        filterDefinitio &= filter.Gt($"{filterPath}.Value", search.SearchTerm);
                        break;
                }
            }

            var (totalcount, data) = await _mongodbContext.Contacts.AggregateByPage<Contact>(
                filterDefinitio,
                Builders<Contact>.Sort.Ascending(c => c.Id),
                (dto.PageIndex - 1) * dto.PageSize,
                dto.PageSize);

            var contacts = data.Select(c => new ContactDTO
            {
                Name = c.CustomAttributes.FirstOrDefault(c => c.Name == FixedAttributes.Name).Value,
                CustomAttributes = c.CustomAttributes.Where(c => c.Name != FixedAttributes.Name).ToList()
            });
            var result = new PaginatedResponse<ContactDTO>(contacts, totalcount, dto.PageIndex, dto.PageSize);

            return new ServiceResult<PaginatedResponse<ContactDTO>>(result);
        }
        public async Task<ServiceResult> DeleteContact(int id)
        {
            Guard.Against.Null(id, nameof(id));
            Guard.Against.NegativeOrZero(id, nameof(id));

            if ((await _mongodbContext.Contacts.DeleteOneAsync(c => c.Id == id)).IsAcknowledged)
                return new();

            return new(false);
        }

        private async Task<bool> CheckUniqeName(ContactDTO contactDto, int id = 0)
        {
            // validate name unique constraint
            var filter = Builders<Contact>.Filter;
            var filterDefinition = filter.ElemMatch(x => x.CustomAttributes,
                att => att.Name == FixedAttributes.Name
                && att.Value == contactDto.Name);

            if (id != 0)
                filterDefinition &= filter.Ne(c => c.Id, id);

            if (await _mongodbContext.Contacts.CountDocumentsAsync(filterDefinition) > 0)
                return false;

            return true;
        }
        private async Task<ServiceResult<Contact>> PrepareEntity(ContactDTO contactDto)
        {
            Guard.Against.Null(contactDto, nameof(contactDto));

            if (!(await CheckUniqeName(contactDto, contactDto.Id)))
                return new ServiceResult<Contact>(default, false, "the contact name already exists!");

            var contact = new Contact();

            if (contactDto.Id != 0)
                contact.Id = contactDto.Id;



            // Validate values types'
            foreach (var attribute in contactDto.CustomAttributes)
            {
                switch (attribute.Type)
                {
                    case AttributeType.Int:
                        if (!int.TryParse(attribute.Value, out int intVal))
                            return new ServiceResult<Contact>(default, false, $"{attribute.Name} value not valid, must be number!");
                        break;
                    case AttributeType.DateTime:
                        if (!attribute.Value.ValidateDate())
                            return new ServiceResult<Contact>(default, false,
                                $"{attribute.Name} value not valid, must be datetime in this format {DateTimeExtention._DateFormat}!");
                        break;
                }
            }

            contact.CustomAttributes = contactDto.CustomAttributes.ToList();

            contact.CustomAttributes.Add(new CustomAttributeRecord(
                   FixedAttributes.Name,
                  contactDto.Name, (int)AttributeType.String));

            if (contactDto.Companies != null && contactDto.Companies.Any())
            {
                var companies = _mongodbContext.Companies.Find(x => contactDto.Companies.Contains(x.Id)).ToList();
                if (companies != null && !companies.Any())
                    return new ServiceResult<Contact>(default, false, "related companies not found!");

                contact.Companies = companies;
            }


            return new ServiceResult<Contact>(contact);
        }
    }
}
