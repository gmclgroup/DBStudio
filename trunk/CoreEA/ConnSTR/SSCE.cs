using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using CoreEA.Args;

namespace CoreEA.ConnSTR
{
    public static partial class DbConnectionString
    {

        /// <summary>
        /// 
        /// </summary>
        public static class SSCE
        {
            /// <summary>
            /// Read Write
            /// Read Only
            /// Shared Read
            /// Exclusive
            /// </summary>
            /// <param name="dbName"></param>
            /// <param name="pwd"></param>
            /// <param name="Encrypt"></param>
            /// <returns></returns>
            public static string GetSSCEConnectionString(string dbName, string pwd, bool Encrypt)
            {
                return GetSSCEConnectionString(dbName, pwd, Encrypt,
                    new OpenModeClass() { mode = OpenMode.ReadOnly, modeDisplayName = "Read Write" }
                    );

            }


            /// <summary>
            /// 
            /// </summary>
            /// <param name="dbFileFullPath"></param>
            /// <param name="pwd"></param>
            /// <param name="Encrypt"></param>
            /// <param name="curOpenMode"></param>
            /// <param name="maxBufferSize">Default is 1024 </param>
            /// <param name="maxDbSize">Default is 256 ,Max is 164091
            /// But we changed to default is 4000</param>
            /// <returns></returns>
            public static string GetSSCEConnectionString(string dbFileFullPath, string pwd, bool Encrypt, OpenModeClass curOpenMode
                ,uint maxDbSize=4000,uint maxBufferSize=1024)
            {
                string result = string.Empty;
                result = string.Format("DataSource={0}", dbFileFullPath);

                if (!string.IsNullOrEmpty(pwd))
                {
                    result = string.Format("{0} ;Password={1}", result, @pwd);
                }

                result = string.Format("{0} ;Encrypt={1}", result, Encrypt);

                switch (curOpenMode.mode)
                {
                    case OpenMode.ReadWrite:
                        result = string.Format("{0}; File Mode={1}", result, curOpenMode.modeDisplayName);
                        break;
                    case OpenMode.ReadOnly:
                        result = string.Format("{0}; File Mode={1};Temp Path=%temp%", result, curOpenMode.modeDisplayName);
                        break;
                    case OpenMode.Exclusive:
                        break;
                    case OpenMode.SharedRead:
                        break;
                    default:
                        result = string.Format("{0}; File Mode={1}", result, curOpenMode.modeDisplayName);
                        break;
                }

                result = string.Format("{0}; Max Database Size={1}", result, maxDbSize);

                if ((maxBufferSize != 0) && (maxBufferSize != 1024))
                {
                    result = string.Format("{0}; Max Buffer Size={1}", result, maxBufferSize);
                }
                return result;
            }
        }

    }
}
