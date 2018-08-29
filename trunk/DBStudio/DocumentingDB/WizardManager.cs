//=============================================================================
//    DBStudio
//    Copyright (C) 2006  ms44

//    This library is free software; you can redistribute it and/or
//    modify it under the terms of the GNU General Public
//    License as published by the Free Software Foundation; either
//    version 2 of the License, or (at your option) any later version.

//    This library is distributed in the hope that it will be useful,
//    but WITHOUT ANY WARRANTY; without even the implied warranty of
//    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
//    Lesser General Public License for more details.

//    You should have received a copy of the GNU Lesser General Public
//    License along with this library; if not, write to the Free Software
//    Foundation, Inc., 59 Temple Place, Suite 330, Boston, MA  02111-1307  USA

//    If you have any questions ,please contact me via 54715112@qq.com
//===============================================================================

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using DBStudio.DataExchangeCenter;
using CoreEA;
using DBStudio.GlobalDefine;
using ETL;

namespace DBStudio.DocumentingDB
{
    public class WizardManager
    {
        List<IStep> Steps;
        int CurrentStepIndex;
        public DocDbObject myDoc = new DocDbObject();

        public WizardManager()
        {
            Steps = new List<IStep>();
            Add(new SelectSrcDbType());
            //This step will be replaced dynamic .
            Add(new SelectSrcDbType());
            Add(new SelectTableWindow());
            Add(new ShowDBSchemaWindow());
            Add(new SelectTargetType());
        }
        public IStep CurrentStep
        {
            get
            {
                return Steps[CurrentStepIndex];
            }
        }
        public IStep FirstStep
        {
            get
            {
                CurrentStepIndex = 0;
                return Steps[CurrentStepIndex];
            }
        }

        public IStep LastStep
        {
            get
            {
                return Steps[Steps.Count-1];
            }
        }

        /// <summary>
        /// If any exception occurred , do not allow next step
        /// This step will check the source database type
        /// </summary>
        public IStep NextStep
        {
            get
            {
                if (CurrentStepIndex < Steps.Count-1)
                {
                    try
                    {
                        #region Match Source Control
                        IStep previousStep = Steps[CurrentStepIndex];
                        if (previousStep.Result is DbTypeWrapper)
                        {
                            IStep curStep = null;
                            switch (((DbTypeWrapper)previousStep.Result).MyType)
                            {
                                case CoreE.UsedDatabaseType.OleDb:
                                    curStep = new SelectSourceDbFile_OleDB();
                                    break;
                                case CoreE.UsedDatabaseType.SqlServer:
                                    curStep = new SelectSqlServerSource();
                                    break;
                                case CoreE.UsedDatabaseType.MySql:
                                    curStep = new SelectMySqlSource();
                                    break;
                                case CoreE.UsedDatabaseType.SqlCE35:
                                    curStep = new SelectSSCEFile();
                                    break;
                                case CoreE.UsedDatabaseType.Effiproz:
                                    curStep = new SelectEffiproz();
                                    break;  
                                default:
                                    break;
                            }
                            curStep.MyDocDataContext = myDoc;
                            Steps[1] = curStep;
                        }
                        #endregion

                        CurrentStep.Leave();

                        CurrentStepIndex++;
                    }
                    catch (Exception ee)
                    {
                        ee.Notify();
                        return null;
                    }

                    return Steps[CurrentStepIndex];
                }
                else
                {
                    return null;
                }

            }
        }

        public bool CanNext
        {
            get
            {
                if (CurrentStepIndex < Steps.Count)
                {
                    if (Steps[CurrentStepIndex] == LastStep)
                    {
                        return false;
                    }

                    if (CurrentStep.CanLeave())
                    {

                    }
                    else
                    {
                        return false;
                    }
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public bool CanPrevious
        {
            get
            {
                if (CurrentStepIndex > 0)
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        public IStep PreviousStep
        {
            get
            {
                if (CurrentStepIndex > 0)
                {
                    CurrentStepIndex--;
                    return Steps[CurrentStepIndex];
                }
                else
                {
                    return null;
                }

            }
        }

        public void Add(IStep step)
        {
            step.MyDocDataContext = myDoc;
            Steps.Add(step);
        }

        public bool Export()
        {
            return myDoc.CurExportor.Export(myDoc.TargetFile,myDoc);
        }

        public void SaveCategory()
        {
            foreach (var item in myDoc.DbObjectList)
            {
                if (!App.MyConfigInfo.DocCategory.Contains(item.Category))
                {
                    App.MyConfigInfo.DocCategory.Add(item.Category);
                }
            }
        }
    }
}
