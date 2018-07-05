using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SQLServerNotifyStream
{
    class ModelProcessedHistory { 
    
        public string HistoryID { get; set; }
        public string ModelElementID { get; set; }
        public string ModelJobID { get; set; }
        public string MaterialID { get; set; }
        public string OrderID { get; set; }
        public string EventDateTime { get; set; }
        public string ProcessStatusID { get; set; }
        public string ProcessLockID { get; set; }
        public string ManufacturerID { get; set; }
        public string ManufacturingProcessID { get; set; }
        public string ModelTransformation { get; set; }
        public string ModelHeight { get; set; }
        public string ModelFilename { get; set; }
        public string ModelVolume { get; set; }
        public string ModelBoundingBoxMin { get; set; }
        public string ModelBoundingBoxMax { get; set; }
        public string Location1 { get; set; }
        public string Location2 { get; set; }
        public string Location3 { get; set; }
        public string Location4 { get; set; }
        public string VirtualItem { get; set; }
        public string ColorID { get; set; }
        public string ModelComment { get; set; }
        public string ModelActive { get; set; }
        public string CreateDate { get; set; }
        public string DeliveryDate { get; set; }
        public string ShippingDate { get; set; }
        public string ReceiveDate { get; set; }
        public string WasSent { get; set; }
        public string Items { get; set; }
        public string ManufName { get; set; }
        public string CAMBlankID { get; set; }
        public string CAMBlankBatchID { get; set; }
        public string CAMJobID { get; set; }
        public string CAMJobName { get; set; }
        public string CAMErrorDescription { get; set; }
        public string ERPItemNo { get; set; }
        public string CacheMaterialName { get; set; }
        public string AltProcessStatusID { get; set; }
        public string CacheColor { get; set; }
        public string ModelElementType { get; set; }
        public string ValidationResult { get; set; }
        public string UnitsCounted { get; set; }
        
    }
}
