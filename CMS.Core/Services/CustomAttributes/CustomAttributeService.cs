using Ardalis.GuardClauses;
using CMS.Core.Entities;
using CMS.Core.Services.CustomAttributes.Dtos;
using CMS.Infrastructure.Data;
using CMS.SharedKernel;
using MongoDB.Driver;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace CMS.Core.Services.CustomAttributes
{
    public class CustomAttributeService : ICustomAttributeService
    {
        private readonly IMongodbContext _mongodbContext;

        public CustomAttributeService(IMongodbContext mongodbContext)
        {
            _mongodbContext = mongodbContext;
        }

        public async Task<ServiceResult<List<EntityCustomAttributeViewDTO>>> GetByEntityType(EntityType entityName)
        {
            var data = await _mongodbContext.EntityCustomAttributes.FindAsync(c =>
            c.EntityType == entityName);

            var result = data.ToList()
                .Select(attr => new EntityCustomAttributeViewDTO
                {
                    Id = attr.Id,
                    Name = attr.Name,
                    Type = attr.Type
                })
                .ToList();

            return new(result);
        }

        public async Task<ServiceResult<List<EntityCustomAttributeFilterDTO>>> GetAllForSearch()
        {
            var data = await _mongodbContext.EntityCustomAttributes.FindAsync(Builders<EntityCustomAttribute>.Filter.Empty);

            var result = data.ToList()
                .Select(attr => new EntityCustomAttributeFilterDTO
                (attr.Id, attr.Name, attr.EntityType, attr.Type))
                .ToList();

            return new(result);
        }


        public async Task<ServiceResult> Create(EntityCustomAttributeDTO entityCustomAttributeDTO)
        {
            var res = await PrepareEntity(entityCustomAttributeDTO);

            if (!res.IsSuccess)
                return res;

            var entityCustomAttribute = res.Data;

            await _mongodbContext.EntityCustomAttributes.InsertOneAsync(entityCustomAttribute);
            return new();
        }

        public async Task<ServiceResult> Update(EntityCustomAttributeDTO dto, string Id)
        {
            Guard.Against.NullOrEmpty(Id, nameof(Id));

            var res = await PrepareEntity(dto, Id);

            if (!res.IsSuccess)
                return new ServiceResult<ServiceResult>(res);

            var replaceResult = (await _mongodbContext.EntityCustomAttributes.ReplaceOneAsync(
                Builders<EntityCustomAttribute>.Filter.Eq(c => c.Id, Id),
                res.Data)).IsAcknowledged;

            if (dto.EntityType == EntityType.Contact)
                _mongodbContext.Contacts.UpdateMany(
                    c => c.CustomAttributes.Any(com => com.Name == dto.Name),
                    Builders<Contact>.Update.Set(x => x.CustomAttributes[-1].Name, res.Data.Name)
                        .Set(x => x.CustomAttributes[-1].Type, res.Data.Type)
                    );

            if (dto.EntityType == EntityType.Company)
                _mongodbContext.Companies.UpdateMany(
                    c => c.CustomAttributes.Any(com => com.Name == dto.Name),
                    Builders<Company>.Update.Set(x => x.CustomAttributes[-1].Name, res.Data.Name)
                        .Set(x => x.CustomAttributes[-1].Type, res.Data.Type)
                    );

            if (!replaceResult)
                return new(false, "update faild!");

            return new();
        }

        public async Task<ServiceResult> Delete(string id)
        {
            Guard.Against.NullOrEmpty(id, nameof(id));

            var attribute = _mongodbContext.EntityCustomAttributes.Find(x => x.Id == id).FirstOrDefault();

            if (attribute == null)
                return new(false, "item not found!");

            if (attribute.EntityType == EntityType.Contact)
                if ((await _mongodbContext.Contacts.FindAsync(
                    Builders<Contact>.Filter.ElemMatch(c => c.CustomAttributes,
                    c => c.Name == attribute.Name))).Any())
                    return new(false, "this attribute is used in contacts, you cannot delete it!");

            if (attribute.EntityType == EntityType.Company)
                if ((await _mongodbContext.Companies.FindAsync(
                    Builders<Company>.Filter.ElemMatch(c => c.CustomAttributes,
                    c => c.Name == attribute.Name))).Any())
                    return new(false, "this attribute is used in companies, you cannot delete it!");

            if (!(await _mongodbContext.EntityCustomAttributes.DeleteOneAsync(c => c.Id == id))
                .IsAcknowledged)
                return new(false);

            return new();
        }

        private async Task<bool> CheckUniqeName(EntityCustomAttributeDTO entityCustomAttributeDTO,
            string id = "")
        {
            // validate name unique constraint
            var filter = Builders<EntityCustomAttribute>.Filter;
            var filterDefinition = filter.And(
                filter.Eq(x => x.Name, entityCustomAttributeDTO.Name),
                filter.Eq(x => x.EntityType, entityCustomAttributeDTO.EntityType)
                );

            if (!string.IsNullOrWhiteSpace(id))
                filterDefinition &= filter.Ne(x => x.Id, id);

            if ((await _mongodbContext.EntityCustomAttributes.CountDocumentsAsync(filterDefinition)) > 0)
                return false;

            return true;
        }

        private async Task<ServiceResult<EntityCustomAttribute>> PrepareEntity(
            EntityCustomAttributeDTO dto,
            string Id = "")
        {
            Guard.Against.Null(dto, nameof(dto));

            if (!await CheckUniqeName(dto, Id))
                return new(default, false, "the entityCustomAttribute name already exists!");

            var entityCustomAttribute = new EntityCustomAttribute
            {
                EntityType = dto.EntityType,
                Name = dto.Name,
                Type = dto.Type
            };

            if (!string.IsNullOrWhiteSpace(Id))
                entityCustomAttribute.Id = Id;

            return new ServiceResult<EntityCustomAttribute>(entityCustomAttribute);
        }
    }
}
