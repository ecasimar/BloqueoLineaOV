using SAPbouiCOM;
using System;

namespace BloqueoLineaOV
{
    internal class BlockLine
    {
        private SAPbouiCOM.Application SBO_Application;

        private void SetApplication()
        {
            SAPbouiCOM.SboGuiApi SboGuiApi = new SAPbouiCOM.SboGuiApi();
            string sConnectionString = "";
            
            sConnectionString = Environment.GetCommandLineArgs().GetValue(1).ToString();

            try
            {
                SboGuiApi.Connect(sConnectionString);
                SBO_Application = SboGuiApi.GetApplication(-1);
            }
            catch (Exception ex)
            {
                System.Windows.Forms.MessageBox.Show($"Error de conexión: {ex.Message}");
                Environment.Exit(0);
            }
        }

        public BlockLine()
        {
            SetApplication();

            SBO_Application.ItemEvent += new _IApplicationEvents_ItemEventEventHandler(SBO_Application_ItemEvent);
            SBO_Application.FormDataEvent += new _IApplicationEvents_FormDataEventEventHandler(SBO_Application_FormDataEvent);

            SBO_Application.AppEvent += new _IApplicationEvents_AppEventEventHandler(SBO_Application_AppEvent);

            SBO_Application.StatusBar.SetText("Add-on de Bloqueo de Líneas iniciado correctamente.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Success);
        }

        private void SBO_Application_AppEvent(BoAppEventTypes EventType)
        {
            if (EventType == BoAppEventTypes.aet_ShutDown || EventType == BoAppEventTypes.aet_ServerTerminition || EventType == BoAppEventTypes.aet_CompanyChanged)
            {
                System.Environment.Exit(0);
            }
        }

        private void SBO_Application_ItemEvent(string FormUID, ref ItemEvent pVal, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                if (pVal.FormTypeEx == "139" && pVal.ItemUID == "38")
                {


                        if ((pVal.EventType == BoEventTypes.et_CLICK ||
                             pVal.EventType == BoEventTypes.et_KEY_DOWN ||
                             pVal.EventType == BoEventTypes.et_COMBO_SELECT ||
                             pVal.EventType == BoEventTypes.et_CHOOSE_FROM_LIST) && pVal.BeforeAction)
                        {
                            Form oForm = SBO_Application.Forms.Item(FormUID);

                            Matrix oMatrix = (Matrix)oForm.Items.Item("38").Specific;

                            if (pVal.Row > 0 && pVal.Row <= oMatrix.RowCount)
                            {
                                string valorEstatus = "";

                                // 1. Obtenemos el objeto específico de la celda de forma genérica usando oMatrix
                                object cellSpecific = oMatrix.Columns.Item("U_CPQ_EC_EstatusWMS").Cells.Item(pVal.Row).Specific;

                                SAPbouiCOM.ComboBox oCombo = (SAPbouiCOM.ComboBox)cellSpecific;
                                if (oCombo.Selected != null)
                                {
                                    valorEstatus = oCombo.Selected.Value.Trim();
                                }
                                


                                if (valorEstatus  != "0")
                                {
                                    SBO_Application.StatusBar.SetText("Esta línea está bloqueada por el estatus WMS.", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Warning);
                                    BubbleEvent = false;
                                    return;
                                }
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText($"Error en ItemEvent: {ex.Message}", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }

        private void SBO_Application_FormDataEvent(ref BusinessObjectInfo BusinessObjectInfo, out bool BubbleEvent)
        {
            BubbleEvent = true;

            try
            {
                // Cuando los datos cambian o se cargan, forzamos un refresco visual si es necesario.
                if (BusinessObjectInfo.FormTypeEx == "139" &&
                    (BusinessObjectInfo.EventType == (SAPbouiCOM.BoEventTypes)BoEventTypes.et_FORM_DATA_LOAD ||
                     BusinessObjectInfo.EventType == (SAPbouiCOM.BoEventTypes)BoEventTypes.et_FORM_DATA_UPDATE) &&
                    !BusinessObjectInfo.BeforeAction)
                {
                    Form oForm = SBO_Application.Forms.Item(BusinessObjectInfo.FormUID);
                    Matrix oMatrix = (Matrix)oForm.Items.Item("38").Specific;
                }
            }
            catch (Exception ex)
            {
                SBO_Application.StatusBar.SetText($"Error en FormDataEvent: {ex.Message}", BoMessageTime.bmt_Short, BoStatusBarMessageType.smt_Error);
            }
        }
    }
}