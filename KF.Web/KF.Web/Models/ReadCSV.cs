using KF.Dto.Modules.Finance;
using KF.Entity;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace KF.Web.Models
{
    public static class ReadCSV
    {
       // [KippinAuthorize]
        public static List<BankExpense> ReadRBCBankStatementFile(HttpPostedFileBase file, int bankid, int userId)
        {
            try
            {
                List<BankExpense> _bankexpense = new List<BankExpense>();
                string AccountType = string.Empty;
                string AccountNumber = string.Empty;
                var reader = new StreamReader(file.InputStream);
                List<string> listA = new List<string>();
                List<string> listB = new List<string>();
                // int index = 0;
                string[] headers = null;

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    headers = line.Split(',');
                    break;
                }

                while (!reader.EndOfStream)
                {
                    var line = reader.ReadLine();
                    var values = line.Split(',');
                    BankExpense _expense = new BankExpense();
                    bool hascolumn = false;
                    for (int i = 0; i < headers.Count(); i++)
                    {
                        if (headers[i].Contains("Account Type"))
                        {
                            _expense.AccountType = values[i];
                            hascolumn = true;
                        }
                        if (headers[i].Contains("Account Number"))
                        {
                            _expense.AccountNumber = values[i];
                            hascolumn = true;
                        }

                        if (headers[i].Contains("Transaction Date"))
                        {
                            _expense.Date = Convert.ToDateTime(values[i]);
                            hascolumn = true;
                        }

                        if (headers[i].Contains("Description 1") || headers[i].Contains("Transaction") || headers[i].Contains("Withdrawal"))
                        {
                            _expense.Description = values[i];
                            hascolumn = true;
                        }

                        if (headers[i].Contains("Debit"))
                        {
                            _expense.Debit = Convert.ToDecimal(string.IsNullOrEmpty(values[i]) ? "0" : values[i]);
                            hascolumn = true;
                        }

                        if (headers[i].Contains("Credit"))
                        {
                            _expense.Credit = Convert.ToDecimal(string.IsNullOrEmpty(values[i]) ? "0" : values[i]);
                            hascolumn = true;
                        }

                        if (headers[i].Contains("Balance"))
                        {
                            _expense.Total = Convert.ToDecimal(string.IsNullOrEmpty(values[i]) ? "0" : values[i]);
                            hascolumn = true;
                        }

                        if (headers[i].Contains("CAD$"))
                        {
                            bool positive = Convert.ToDecimal(values[i]) > 0;
                            if (positive)
                                _expense.Credit = Convert.ToDecimal(values[i] ?? "0");
                            else
                                _expense.Debit = values[i] == null ? Convert.ToDecimal("0") : Math.Abs(Convert.ToDecimal(values[i]));

                            hascolumn = true;
                        }

                        if (headers[i].Contains("AccountName"))
                        {
                            _expense.AccountName = string.IsNullOrEmpty(values[i]) ? null : values[i];
                            hascolumn = true;
                        }


                    }
                    if (hascolumn)
                    {
                        //overide by ankush 23th jan 2016
                        if (userId == 0)
                        {
                            var _user = UserData.GetCurrentUserData();
                            //  UserRepository _rep = new UserRepository();

                            _expense.UserId = _user.Id;
                            //by default we are set bank id =1 but is shoud be change according to bank links
                            _expense.BankId = bankid;
                            _expense.CreatedDate = DateTime.Now;
                            if (_expense.ClassificationId == null)
                            {
                                _expense.ClassificationId = 1;
                            }
                            if (_expense.CategoryId == null)
                            {
                                _expense.CategoryId = 6;
                            }

                        }
                        else
                        {
                            _expense.UserId = userId;
                            _expense.BankId = bankid;
                            _expense.CreatedDate = DateTime.Now;
                            if (_expense.ClassificationId == null)
                            {
                                _expense.ClassificationId = 1;
                            }
                            if (_expense.CategoryId == null)
                            {
                                _expense.CategoryId = 6;
                            }
                        }
                    }

                    _bankexpense.Add(_expense);
                    hascolumn = false;
                }
                foreach (var item in _bankexpense)
                {
                    if (item.Credit == null)
                    {
                        item.Credit = Convert.ToDecimal(0);
                    }
                    if (item.Debit == null)
                    {
                        item.Debit = Convert.ToDecimal(0);
                    }
                    if (item.StatusId == null)
                    {
                        item.StatusId = 1;
                    }
                    if (item.Total == null)
                    {
                        item.Total = Convert.ToDecimal(0);
                    }
                }
                _bankexpense.ForEach(a => a.UploadedStatementName = file.FileName);
                return _bankexpense;

            }
            catch (Exception ex)
            {
                return null;
            }

        }

        public static DateTime ReadLatestDate(HttpPostedFileBase file, int bankid, string bankname)
        {
            DateTime dt = new DateTime();
            List<DateTime> dtAll = new List<DateTime>();
            try
            {
                var csvReader = new StreamReader(file.InputStream);
                string inputDataRead;
                var values = new List<string>();
                while ((inputDataRead = csvReader.ReadLine()) != null)
                {
                    values.Add(inputDataRead.Trim().Replace(" ", "").Replace(",", " "));
                }
                if (bankid != 2)
                {
                    values.Remove(values[0]);
                }
                //For removing header
                //values.Remove(values[0]);
                //values.Remove(values[values.Count - 1]);
                foreach (var value in values)
                {
                    if (value != "")
                    {
                        var eachValue = value.Split(' ');
                        if (bankid == 1)
                        {
                            if (!string.IsNullOrEmpty(eachValue[2]))
                            {
                                string date = eachValue[2];
                                dtAll.Add(Convert.ToDateTime(date));
                            }
                        }
                        if (bankid == 2)
                        {
                            if (!string.IsNullOrEmpty(eachValue[0]))
                            {
                                string date = eachValue[0];
                                dtAll.Add(Convert.ToDateTime(date));
                            }
                        }
                        if (bankid == 3)
                        {
                            if (!string.IsNullOrEmpty(eachValue[2]))
                            {
                                string date = eachValue[2];
                                dtAll.Add(DateTime.ParseExact(date, "yyyyMMdd", null));
                            }
                        }
                        if (bankid == 4)
                        {
                            if (!string.IsNullOrEmpty(eachValue[0]))
                            {
                                string date = eachValue[0];
                                dtAll.Add(Convert.ToDateTime(date));
                            }
                        }
                        if (bankid == 5)
                        {
                            if (!string.IsNullOrEmpty(eachValue[0]))
                            {
                                string date = eachValue[0];
                                dtAll.Add(Convert.ToDateTime(date));
                            }
                        }
                        else
                        {
                            if (bankname.ToLower().Contains("scotia"))
                            {
                                if (!string.IsNullOrEmpty(eachValue[0]))
                                {
                                    string date = eachValue[0];
                                    dtAll.Add(Convert.ToDateTime(date));
                                }
                            }
                            else if (bankname.ToLower().Contains("bmo"))
                            {
                                if (!string.IsNullOrEmpty(eachValue[2]))
                                {
                                    string date = eachValue[2];
                                    dtAll.Add(DateTime.ParseExact(date, "yyyyMMdd", null));
                                }
                            }
                            else if (bankname.ToLower().Contains("td"))
                            {
                                if (!string.IsNullOrEmpty(eachValue[0]))
                                {
                                    string date = eachValue[0];
                                    dtAll.Add(Convert.ToDateTime(date));
                                }
                            }
                            else if (bankname.ToLower().Contains("cibc"))
                            {
                                if (!string.IsNullOrEmpty(eachValue[0]))
                                {
                                    string date = eachValue[0];
                                    dtAll.Add(Convert.ToDateTime(date));
                                }
                            }
                        }

                    }
                }
                dt = dtAll.OrderByDescending(i => i.Date).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
            return dt;
        }

        public static string ReadRbcLatestDate(HttpPostedFileBase file, int bankid, string bankname)
        {
            DateTime dt = new DateTime();
            string AccNo = string.Empty;
            List<DateTime> dtAll = new List<DateTime>();
            try
            {
                var csvReader = new StreamReader(file.InputStream);
                string inputDataRead;
                var values = new List<string>();
                while ((inputDataRead = csvReader.ReadLine()) != null)
                {
                    values.Add(inputDataRead.Trim().Replace(" ", "").Replace(",", " "));
                }
                if (bankid != 2)
                {
                    values.Remove(values[0]);
                }
                //For removing header
                //values.Remove(values[0]);
                //values.Remove(values[values.Count - 1]);
                foreach (var value in values)
                {
                    if (value != "")
                    {
                        var eachValue = value.Split(' ');
                        if (bankid == 1)
                        {
                            if (!string.IsNullOrEmpty(eachValue[2]))
                            {
                                string date = eachValue[2];
                                dtAll.Add(Convert.ToDateTime(date));
                            }
                            if (!string.IsNullOrEmpty(eachValue[1]))
                            {
                                string accNo = eachValue[1];
                                AccNo = accNo;
                            }
                        }
                        else
                        {
                            if (bankname.ToLower().Contains("royal"))
                            {

                                if (!string.IsNullOrEmpty(eachValue[2]))
                                {
                                    string date = eachValue[2];
                                    dtAll.Add(Convert.ToDateTime(date));
                                }
                                if (!string.IsNullOrEmpty(eachValue[1]))
                                {
                                    string accNo = eachValue[1];
                                    AccNo = accNo;
                                }
                            }
                        }

                    }
                }
                dt = dtAll.OrderByDescending(i => i.Date).FirstOrDefault();
            }
            catch (Exception)
            {

                throw;
            }
            return dt.ToShortDateString() + "^" + AccNo;
        }

        public static List<ReadScotiaBankDetails> GetScotiaBankData(HttpPostedFileBase file, int bankid, int userId, int classificationId, int CategoryId)
        {

            var csvReader = new StreamReader(file.InputStream);
            var uploadModelList = new List<ReadScotiaBankDetails>();
            string inputDataRead;
            var values = new List<string>();
            while ((inputDataRead = csvReader.ReadLine()) != null)
            {
                values.Add(inputDataRead.Trim().Replace(" ", "").Replace(",", " "));
            }
            foreach (var value in values)
            {
                if (value != "")
                {
                    var uploadModelRecord = new ReadScotiaBankDetails();
                    var eachValue = value.Split(' ');

                    if (!string.IsNullOrEmpty(eachValue[0]))
                    {
                        string date = eachValue[0];
                        DateTime dt = Convert.ToDateTime(date);
                        uploadModelRecord.Date = dt;
                    }


                    if (!string.IsNullOrEmpty(eachValue[1]))
                    {
                        if (eachValue[1].Contains("-"))
                        {
                            //Debit
                            uploadModelRecord.Debit = eachValue[1].Replace('-', ' ');
                            uploadModelRecord.Credit = "0.00";
                        }
                        else
                        {
                            //Credit
                            uploadModelRecord.Credit = eachValue[1];
                            uploadModelRecord.Debit = "0.00";
                        }
                    }
                    else
                    {
                        uploadModelRecord.Credit = "0.00";
                        uploadModelRecord.Debit = "0.00";
                    }

                    uploadModelRecord.Description = eachValue[3] != "" ? eachValue[3] : string.Empty;
                    uploadModelRecord.Company = eachValue[4] != "" ? eachValue[4] : string.Empty;
                    uploadModelRecord.UserId = userId;
                    uploadModelRecord.ClassificationId = classificationId;
                    uploadModelRecord.CategoryId = CategoryId;
                    uploadModelRecord.BankId = bankid;
                    uploadModelList.Add(uploadModelRecord);
                }
            }

            return uploadModelList;
        }

        public static List<ReadTdDetails> GetTdBankData(HttpPostedFileBase file, int bankid, int userId, int classificationId, int CategoryId)
        {
            var csvReader = new StreamReader(file.InputStream);
            var uploadModelList = new List<ReadTdDetails>();
            string inputDataRead;
            var values = new List<string>();
            while ((inputDataRead = csvReader.ReadLine()) != null)
            {
                values.Add(inputDataRead.Trim().Replace(" ", "").Replace(",", " "));
            }
            //For removing header
            values.Remove(values[0]);
            // values.Remove(values[values.Count - 1]);
            foreach (var value in values)
            {
                if (value != "")
                {
                    var uploadModelRecord = new ReadTdDetails();
                    var eachValue = value.Split(' ');
                    if (!string.IsNullOrEmpty(eachValue[0]))
                    {
                        string date = eachValue[0];
                        DateTime dt = Convert.ToDateTime(date);
                        uploadModelRecord.Date = dt;
                    }
                    // uploadModelRecord.Date = eachValue[0] != "" ? eachValue[0] : string.Empty;
                    uploadModelRecord.Withdrawal = eachValue[1] != "" ? eachValue[1] : string.Empty;
                    uploadModelRecord.Debit = eachValue[2] != "" ? eachValue[2] : "0.00";
                    uploadModelRecord.Credit = eachValue[3] != "" ? eachValue[3] : "0.00";
                    uploadModelRecord.Balance = eachValue[4] != "" ? eachValue[4] : "0.00";
                    uploadModelRecord.UserId = userId;
                    uploadModelRecord.ClassificationId = classificationId;
                    uploadModelRecord.CategoryId = CategoryId;
                    uploadModelRecord.BankId = bankid;
                    uploadModelList.Add(uploadModelRecord);
                }
            }

            return uploadModelList;
        }

        public static List<ReadCisvDetails> GetCibcBankData(HttpPostedFileBase file, int bankid, int userId, int classificationId, int CategoryId)
        {
            var csvReader = new StreamReader(file.InputStream);
            var uploadModelList = new List<ReadCisvDetails>();
            string inputDataRead;
            var values = new List<string>();
            while ((inputDataRead = csvReader.ReadLine()) != null)
            {
                values.Add(inputDataRead.Trim().Replace(" ", "").Replace(",", " "));
            }
            //For removing header
            values.Remove(values[0]);
            // values.Remove(values[values.Count - 1]);
            foreach (var value in values)
            {
                if (value != "")
                {
                    var uploadModelRecord = new ReadCisvDetails();
                    var eachValue = value.Split(' ');
                    if (!string.IsNullOrEmpty(eachValue[0]))
                    {
                        string date = eachValue[0];
                        DateTime dt = Convert.ToDateTime(date);
                        uploadModelRecord.Date = dt;
                    }

                    uploadModelRecord.Transaction = eachValue[1] != "" ? eachValue[1] : string.Empty;
                    uploadModelRecord.Debit = eachValue[2] != "" ? eachValue[2] : "0.00";
                    uploadModelRecord.Credit = eachValue[3] != "" ? eachValue[3] : "0.00";
                    uploadModelRecord.UserId = userId;
                    uploadModelRecord.ClassificationId = classificationId;
                    uploadModelRecord.CategoryId = CategoryId;
                    uploadModelRecord.BankId = bankid;
                    uploadModelList.Add(uploadModelRecord);
                }
            }

            return uploadModelList;
        }

        public static List<ReadBmoBankDetails> GetBmoBankData(HttpPostedFileBase file, int bankid, int userId, int classificationId, int CategoryId)
        {

            var csvReader = new StreamReader(file.InputStream);
            var uploadModelList = new List<ReadBmoBankDetails>();
            string inputDataRead;
            var values = new List<string>();
            while ((inputDataRead = csvReader.ReadLine()) != null)
            {
                values.Add(inputDataRead.Trim().Replace(" ", "").Replace(",", " "));
            }
            //For removing header
            values.Remove(values[0]);
            //values.Remove(values[values.Count - 1]);
            foreach (var value in values)
            {
                if (value != "")
                {
                    var uploadModelRecord = new ReadBmoBankDetails();
                    var eachValue = value.Split(' ');

                    if (!string.IsNullOrEmpty(eachValue[2]))
                    {
                        string date = eachValue[2];
                        DateTime dt = DateTime.ParseExact(date, "yyyyMMdd", null);
                        uploadModelRecord.Date = dt;
                    }


                    if (!string.IsNullOrEmpty(eachValue[1]))
                    {
                        if (eachValue[1].Contains("CREDIT"))
                        {
                            //Debit
                            uploadModelRecord.Debit = "0.00";
                            uploadModelRecord.Credit = eachValue[3].ToString();
                        }
                        else
                        {
                            //Credit
                            uploadModelRecord.Credit = "0.00";
                            uploadModelRecord.Debit = eachValue[3].ToString();
                        }
                    }
                    else
                    {
                        uploadModelRecord.Credit = "0.00";
                        uploadModelRecord.Debit = "0.00";
                    }

                    uploadModelRecord.Description = eachValue[4] != "" ? eachValue[4] : string.Empty;
                    uploadModelRecord.AccountNo = eachValue[0] != "" ? eachValue[0].Replace("'", " ").Trim() : string.Empty;
                    uploadModelRecord.UserId = userId;
                    uploadModelRecord.ClassificationId = classificationId;
                    uploadModelRecord.CategoryId = CategoryId;
                    uploadModelRecord.BankId = bankid;
                    uploadModelList.Add(uploadModelRecord);
                }
            }

            return uploadModelList;
        }

    }
}