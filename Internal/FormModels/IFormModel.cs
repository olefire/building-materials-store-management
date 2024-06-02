using System.Collections.Generic;
using BuildingMaterialsStoreManagement.Internal.Database.Abstract;
using BuildingMaterialsStoreManagement.Internal.Utils;

namespace BuildingMaterialsStoreManagement.Internal.FormModels
{
    public interface IFormModel
    {
        List<TableUtils.FormField> CreateFormFieldList(PairRepository pairRepository = null, ItemRepository itemRepository = null);
    }
}