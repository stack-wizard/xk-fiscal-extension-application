using Micros.Ops;
using System.Collections.Generic;

namespace Mikos.XK.Fiscal.Util
{
    public class OpsCommandUtil
    {
        public static void retransmitCheck(OpsContext opsContext, string checkGuid)
        {
            OpsCommand openCheckByGuid = new OpsCommand(OpsCommandType.PickUpCheckByGuid);
            openCheckByGuid.Data = checkGuid.Trim();
            OpsCommand payment = new OpsCommand(OpsCommandType.Payment);
            payment.Number = 150;
            List<OpsCommand> data = new List<OpsCommand> { openCheckByGuid };
            OpsCommand command = new OpsCommand(OpsCommandType.Macro)
            {
                Data = data
            };
            opsContext.ProcessCommand(command);
        }

        public static void AdjustClosedCheck(OpsContext opsContext, string checkGuid)
        {
            OpsCommand openCheckByGuid = new OpsCommand(OpsCommandType.AdjustClosedCheckByGuid);
            openCheckByGuid.Data = checkGuid.Trim();
            opsContext.ProcessCommand(openCheckByGuid);
        }

        public static void TransactionCancel(OpsContext opsContext)
        {
            OpsCommand item = new OpsCommand(OpsCommandType.TransactionCancel);
            OpsCommand item2 = new OpsCommand(OpsCommandType.EnterKey);
            List<OpsCommand> data = new List<OpsCommand> { item, item2 };
            OpsCommand command = new OpsCommand(OpsCommandType.Macro)
            {
                Data = data
            };
            opsContext.ProcessCommand(command);
        }

        public static void VoidClosedCheck(OpsContext opsContext, string chkGuid)
        {
            OpsCommand voidCheckByGuid = new OpsCommand(OpsCommandType.VoidClosedCheckByGuid)
            {
                Data = chkGuid.Trim()
            };
            OpsCommand dialogOk = new OpsCommand(OpsCommandType.DialogOk);
            List<OpsCommand> data = new List<OpsCommand> { voidCheckByGuid, dialogOk };

            OpsCommand command = new OpsCommand(OpsCommandType.Macro)
            {
                Data = data
            };

            opsContext.ProcessCommand(command);
        }

        public static void ReprintCheckByGuid(OpsContext opsContext, string chkGuid)
        {
            if (string.IsNullOrWhiteSpace(chkGuid)) return;

            var cmd = new OpsCommand(OpsCommandType.ReprintClosedCheckByGuid)
            {
                Data = chkGuid.Trim()
            };
            opsContext.ProcessCommand(cmd);
        }
    }
}
