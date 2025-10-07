//using Model.GenericUAServer;
using Opc.Ua;
using System;
using System.Data.Entity.Migrations;
using System.Linq;

using Model.GenericUAServer;

namespace DB.ModelLib.Managers
{
    public class ProcessModelContext
    {
        public ReadingsData RData { get; set; }
        public bool m_db_upddate = false;

        public ProcessModelContext() 
        {
            RData = new ReadingsData();
        }

        public void UpdateReadingsDataMemory(string NodeIdentifier, DataValue value)
        {
            // Check the results to make sure the write succeeded
            if (value.StatusCode != Opc.Ua.StatusCodes.Good)
            {
                Utils.Trace("Error: failed to update readings data im memory");
            }
            else
            {
                switch (NodeIdentifier)
                {
                    #region Folder 1

                    case "BoolFieldName1":
                        RData.BoolFieldName1 = (bool)value.Value;
                        break;
                    case "StringFieldName1":
                        RData.StringFieldName1 = (string)value.Value;
                        break;
                    case "ByteFieldName1":
                        RData.ByteFieldName1 = (string)value.Value;
                        break;
                    case "ByteStringFieldName1":
                        RData.ByteStringFieldName1 = (string)value.Value;
                        break;
                    case "DateTimeFieldName1":
                        RData.DateTimeFieldName1 = (DateTime)value.Value;
                        break;
                    case "DoubleFieldName1":
                        RData.DoubleFieldName1 = (Double)value.Value;
                        break;

                    #endregion

                    #region Folder 2

                    case "BoolFieldName2":
                        RData.BoolFieldName2 = (bool)value.Value;
                        break;
                    case "StringFieldName2":
                        RData.StringFieldName2 = (string)value.Value;
                        break;
                    case "ByteFieldName2":
                        RData.ByteFieldName2 = (string)value.Value;
                        break;
                    case "ByteStringFieldName2":
                        RData.ByteStringFieldName2 = (string)value.Value;
                        break;
                    case "DateTimeFieldName2":
                        RData.DateTimeFieldName2 = (DateTime)value.Value;
                        break;
                    case "DoubleFieldName2":
                        RData.DoubleFieldName2 = (Double)value.Value;
                        break;

                    #endregion

                    #region Folder 3

                    case "BoolFieldName3":
                        RData.BoolFieldName3 = (bool)value.Value;
                        break;
                    case "StringFieldName3":
                        RData.StringFieldName3 = (string)value.Value;
                        break;
                    case "ByteFieldName3":
                        RData.ByteFieldName3 = (string)value.Value;
                        break;
                    case "ByteStringFieldName3":
                        RData.ByteStringFieldName3 = (string)value.Value;
                        break;
                    case "DateTimeFieldName3":
                        RData.DateTimeFieldName3 = (DateTime)value.Value;
                        break;
                    case "DoubleFieldName3":
                        RData.DoubleFieldName3 = (Double)value.Value;
                        break;

                    #endregion

                    #region Folder 4 

                    case "BoolFieldName4":
                        RData.BoolFieldName4 = (bool)value.Value;
                        break;
                    case "StringFieldName4":
                        RData.StringFieldName4 = (string)value.Value;
                        break;
                    case "ByteFieldName4":
                        RData.ByteFieldName4 = (string)value.Value;
                        break;
                    case "ByteStringFieldName4":
                        RData.ByteStringFieldName4 = (string)value.Value;
                        break;
                    case "DateTimeFieldName4":
                        RData.DateTimeFieldName4 = (DateTime)value.Value;
                        break;
                    case "DoubleFieldName4":
                        RData.DoubleFieldName4 = (Double)value.Value;
                        break;

                    #endregion

                    #region Folder 5

                    case "BoolFieldName5":
                        RData.BoolFieldName5 = (bool)value.Value;
                        break;
                    case "StringFieldName5":
                        RData.StringFieldName5 = (string)value.Value;
                        break;
                    case "ByteFieldName5":
                        RData.ByteFieldName5 = (string)value.Value;
                        break;
                    case "ByteStringFieldName5":
                        RData.ByteStringFieldName5 = (string)value.Value;
                        break;
                    case "DateTimeFieldName5":
                        RData.DateTimeFieldName5 = (DateTime)value.Value;
                        break;
                    case "DoubleFieldName5":
                        RData.DoubleFieldName5 = (Double)value.Value;
                        break;

                    #endregion

                    #region Folder 6

                    case "BoolFieldName6":
                        RData.BoolFieldName6 = (bool)value.Value;
                        break;
                    case "StringFieldName6":
                        RData.StringFieldName6 = (string)value.Value;
                        break;
                    case "ByteFieldName6":
                        RData.ByteFieldName6 = (string)value.Value;
                        break;
                    case "ByteStringFieldName6":
                        RData.ByteStringFieldName6 = (string)value.Value;
                        break;
                    case "DateTimeFieldName6":
                        RData.DateTimeFieldName6 = (DateTime)value.Value;
                        break;
                    case "DoubleFieldName6":
                        RData.DoubleFieldName6 = (Double)value.Value;
                        break;


                    #endregion                        
                }
            }

            m_db_upddate = true;
        }

        public void InsertReadingsData()
        {
            if (m_db_upddate)
            {
                using (var db = new GenericUAServerEntities())
                {
                    try
                    {
                        ReadingsData db_data = db.ReadingsData.Where(r => r.idProcess == RData.idProcess).FirstOrDefault();

                        if (db_data == null)
                        {
                            RData.processStartDate = DateTime.Now;
                            db.ReadingsData.Add(RData);
                        }
                        else
                        {
                            db.ReadingsData.AddOrUpdate(RData);
                        }

                        db.SaveChanges();
                    }
                    catch (Exception ex)
                    {
                        Utils.Trace("Error: failed to complete database operations. " + ex.ToString());
                    }
                }
            }
        }              

        //public async Task<IEnumerable<ReadingsData>> GetReadingsData()
        //{
        //    using (var db = new GenericUAServerDBEntities())
        //    {   
        //        return (IEnumerable<ReadingsData>)db.ReadingsData.ToListAsync<ReadingsData>();
        //    }
        //}
    }
}
