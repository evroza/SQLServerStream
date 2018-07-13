using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TableDependency;
using TableDependency.SqlClient;
using TableDependency.EventArgs;
using System.Data.SqlClient;
using System.Data;

namespace SQLServerNotifyStream.JSONConvert
{
    class JSONConverter
    {

        public static JObject EntityToJObject(RecordChangedEventArgs<ModelProcessedHistory> e)
        {
            var changedEntity = e.Entity;

            dynamic record = new JObject();
            record.dataset = new JObject();

            record.dataset.HistoryID = changedEntity.HistoryID;
            record.dataset.ModelElementID = changedEntity.ModelElementID;
            record.dataset.ModelJobID = changedEntity.ModelJobID;
            record.dataset.MaterialID = changedEntity.MaterialID;
            record.dataset.OrderID = changedEntity.OrderID;
            record.dataset.EventDateTime = changedEntity.EventDateTime;
            record.dataset.ProcessStatusID = changedEntity.ProcessStatusID;
            record.dataset.ProcessLockID = changedEntity.ProcessLockID;
            record.dataset.ManufacturerID = changedEntity.ManufacturerID;
            record.dataset.ManufacturingProcessID = changedEntity.ManufacturingProcessID;
            record.dataset.ModelTransformation = changedEntity.ModelTransformation;
            record.dataset.ModelHeight = changedEntity.ModelHeight;
            record.dataset.ModelFilename = changedEntity.ModelFilename;
            record.dataset.ModelVolume = changedEntity.ModelVolume;
            record.dataset.ModelBoundingBoxMin = changedEntity.ModelBoundingBoxMin;
            record.dataset.ModelBoundingBoxMax = changedEntity.ModelBoundingBoxMax;
            record.dataset.Location1 = changedEntity.Location1;
            record.dataset.Location2 = changedEntity.Location2;
            record.dataset.Location3 = changedEntity.Location3;
            record.dataset.Location4 = changedEntity.Location4;
            record.dataset.VirtualItem = changedEntity.VirtualItem;
            record.dataset.ColorID = changedEntity.ColorID;
            record.dataset.ModelComment = changedEntity.ModelComment;
            record.dataset.ModelActive = changedEntity.ModelActive;
            record.dataset.CreateDate = changedEntity.CreateDate;
            record.dataset.DeliveryDate = changedEntity.DeliveryDate;
            record.dataset.ShippingDate = changedEntity.ShippingDate;
            record.dataset.ReceiveDate = changedEntity.ReceiveDate;
            record.dataset.WasSent = changedEntity.WasSent;
            record.dataset.Items = changedEntity.Items;
            record.dataset.ManufName = changedEntity.ManufName;
            record.dataset.CAMBlankID = changedEntity.CAMBlankID;
            record.dataset.CAMBlankBatchID = changedEntity.CAMBlankBatchID;
            record.dataset.CAMJobID = changedEntity.CAMJobID;
            record.dataset.CAMJobName = changedEntity.CAMJobName;
            record.dataset.CAMErrorDescription = changedEntity.CAMErrorDescription;
            record.dataset.ERPItemNo = changedEntity.ERPItemNo;
            record.dataset.CacheMaterialName = changedEntity.CacheMaterialName;
            record.dataset.AltProcessStatusID = changedEntity.AltProcessStatusID;
            record.dataset.CacheColor = changedEntity.CacheColor;
            record.dataset.ModelElementType = changedEntity.ModelElementType;
            record.dataset.ValidationResult = changedEntity.ValidationResult;
            record.dataset.UnitsCounted = changedEntity.UnitsCounted;

            if (record.dataset.UnitsCounted.Equals('0'))
            {
                // Reconcile in DB and return null for this record
                __ValidateUnitsCounted(record.dataset.HistoryID);
                return null;
            }


            return record;

        }


        public static JObject SqlDataReaderToJObject(SqlDataReader reader)
        {
            dynamic record = new JObject();
            record.dataset = new JObject();

            record.dataset.HistoryID = reader["HistoryID"].ToString();
            record.dataset.ModelElementID = reader["ModelElementID"].ToString();
            record.dataset.ModelJobID = reader["ModelJobID"].ToString();
            record.dataset.MaterialID = reader["MaterialID"].ToString();
            record.dataset.OrderID = reader["OrderID"].ToString();
            record.dataset.EventDateTime = reader["EventDateTime"].ToString();
            record.dataset.ProcessStatusID = reader["ProcessStatusID"].ToString();
            record.dataset.ProcessLockID = reader["ProcessLockID"].ToString();
            record.dataset.ManufacturerID = reader["ManufacturerID"].ToString();
            record.dataset.ManufacturingProcessID = reader["ManufacturingProcessID"].ToString();
            record.dataset.ModelTransformation = reader["ModelTransformation"].ToString();
            record.dataset.ModelHeight = reader["ModelHeight"].ToString();
            record.dataset.ModelFilename = reader["ModelFilename"].ToString();
            record.dataset.ModelVolume = reader["ModelVolume"].ToString();
            record.dataset.ModelBoundingBoxMin = reader["ModelBoundingBoxMin"].ToString();
            record.dataset.ModelBoundingBoxMax = reader["ModelBoundingBoxMax"].ToString();
            record.dataset.Location1 = reader["Location1"].ToString();
            record.dataset.Location2 = reader["Location2"].ToString();
            record.dataset.Location3 = reader["Location3"].ToString();
            record.dataset.Location4 = reader["Location4"].ToString();
            record.dataset.VirtualItem = reader["VirtualItem"].ToString();
            record.dataset.ColorID = reader["ColorID"].ToString();
            record.dataset.ModelComment = reader["ModelComment"].ToString();
            record.dataset.ModelActive = reader["ModelActive"].ToString();
            record.dataset.CreateDate = reader["CreateDate"].ToString();
            record.dataset.DeliveryDate = reader["DeliveryDate"].ToString();
            record.dataset.ShippingDate = reader["ShippingDate"].ToString();
            record.dataset.ReceiveDate = reader["ReceiveDate"].ToString();
            record.dataset.WasSent = reader["WasSent"].ToString();
            record.dataset.Items = reader["Items"].ToString();
            record.dataset.ManufName = reader["ManufName"].ToString();
            record.dataset.CAMBlankID = reader["CAMBlankID"].ToString();
            record.dataset.CAMBlankBatchID = reader["CAMBlankBatchID"].ToString();
            record.dataset.CAMJobID = reader["CAMJobID"].ToString();
            record.dataset.CAMJobName = reader["CAMJobName"].ToString();
            record.dataset.CAMErrorDescription = reader["CAMErrorDescription"].ToString();
            record.dataset.ERPItemNo = reader["ERPItemNo"].ToString();
            record.dataset.CacheMaterialName = reader["CacheMaterialName"].ToString();
            record.dataset.AltProcessStatusID = reader["AltProcessStatusID"].ToString();
            record.dataset.CacheColor = reader["CacheColor"].ToString();
            record.dataset.ModelElementType = reader["ModelElementType"].ToString();
            record.dataset.ValidationResult = reader["ValidationResult"].ToString();
            record.dataset.UnitsCounted = reader["UnitsCounted"].ToString();


            // Need to verify units counted isn't zero -- cannot be zero
            // Might sometimes be zero because of race condition in table update, so we need to correct this

            if (record.dataset.UnitsCounted.Equals('0'))
            {
                // Reconcile in DB and return null for this record
                __ValidateUnitsCounted(record.dataset.HistoryID);
                return null;
            }

            return record;
        }


        private static void __ValidateUnitsCounted(string historyID)
        {
            // Private method to check whether the UnitsCounted Field is 0 in a record processed by this class
            // If zero, then the record is possibly
            using (SqlConnection con = new SqlConnection(Globals.DBConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("Proc_reconcileProcessedHistory", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.Add("@HistoryID", SqlDbType.VarChar).Value = historyID;

                    con.Open();
                    cmd.ExecuteNonQuery();
                }
            }
        }

    }
}
