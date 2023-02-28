using Autodesk.Revit.Attributes;
using Autodesk.Revit.DB;
using Autodesk.Revit.DB.Architecture;
using Autodesk.Revit.DB.Mechanical;
using Autodesk.Revit.DB.Plumbing;
using Autodesk.Revit.DB.Structure;
using Autodesk.Revit.UI;
using Autodesk.Revit.UI.Selection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CreationRoomTag
{
    [Transaction(TransactionMode.Manual)]
    
    public class CreationRoomTag : IExternalCommand
    {
        public Result Execute(ExternalCommandData commandData, ref string message, ElementSet elements)
        {
            Document doc = commandData.Application.ActiveUIDocument.Document;
            List<Level> listLevel = new FilteredElementCollector(doc)
                .OfClass(typeof(Level))
                .OfType<Level>()
                .ToList();

            Transaction ts = new Transaction(doc, "Помещения");
            ts.Start();
            ICollection<ElementId> rooms;
            foreach (Level level in listLevel)
            {
                rooms = doc.Create.NewRooms2(level);
            }

            //FilteredElementCollector tagsCollector = new FilteredElementCollector(doc)
            //        .OfCategory(BuiltInCategory.OST_RoomTags)
            //        .WhereElementIsNotElementType();

            List<RoomTag> tagsCollector = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_RoomTags)
                .WhereElementIsNotElementType()
                .Cast<RoomTag>()
                .ToList();

            RoomTagType roomTagType = new FilteredElementCollector(doc)
                .OfCategory(BuiltInCategory.OST_RoomTags)
                .OfType<RoomTagType>()
                .Where(x => x.Name.Equals("Room Tag"))
                .FirstOrDefault();
           
            foreach (RoomTag roomTag in tagsCollector)
            {                
                roomTag.ChangeTypeId(roomTagType.Id);
            }            
            ts.Commit();

            return Result.Succeeded;
        }
    }
}
