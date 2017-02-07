using MODI;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace KF.Utilities.Finance
{
    public static class ReadDateFromImage
    {
        public static DateTime ReadTextFromImage(string imagePath)
        {
            try
            {
                Document modiDocument = new Document();
                modiDocument.Create(imagePath);
                modiDocument.OCR(MiLANGUAGES.miLANG_ENGLISH);
                MODI.Image modiImage = (modiDocument.Images[0] as MODI.Image);

                // 1.Get text from image
                string extractedText = (modiImage.Layout.Text).ToLower();


                modiDocument.Close();

                return GetDate(extractedText.Replace("\r\n", " "));
            }
            catch (Exception)
            {
                return DateTime.Now;
            }
        }

        public static DateTime GetDate(string OcrData)
        {
            Boolean hasDate = false;
            DateTime dateTime = new DateTime();
            String[] inputText = OcrData.Split(' ');//split on a whitespace

            foreach (String text in inputText)
            {
                //Use the Parse() method
                try
                {
                    dateTime = DateTime.ParseExact(text, "dd/MM/yyyy", CultureInfo.InvariantCulture);
                    hasDate = true;
                    break;//no need to execute/loop further if you have your date
                }
                catch (Exception)
                {

                }
            }

            //after breaking from the foreach loop, you can check if hasDate=true
            //if it is, then your user entered a date and you can retrieve it from the dateTime 

            if (hasDate)
            {
                return dateTime;

                //user entered a date, get it from dateTime
            }
            else
            {
                return DateTime.Now;
                //user didn't enter any date
            }
        }
    }
}
