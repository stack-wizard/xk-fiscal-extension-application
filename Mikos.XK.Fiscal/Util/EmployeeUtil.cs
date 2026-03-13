using Micros.Ops;
using Micros.PosCore.Extensibility.DataStore;
using Micros.PosCore.Extensibility.DataStore.DbRecords;
using static Mikos.XK.Fiscal.Application;

namespace Mikos.XK.Fiscal.Util
{
    public class EmployeeUtil
    {
        public static string GetEmployeeId(OpsContext opsContext, DataStoreClient dataStore, string ApplicationName)
        {
            int transEmployeeNumber = opsContext.TransEmployeeNumber;
            DbEmployee transactionEmployee = dataStore.ReadEmployeeByNum(transEmployeeNumber);
            bool usePayrollId = FiscalConfigUtil.UsePayrollId(opsContext, dataStore, ApplicationName);
            string employeeId = getEmpId(opsContext, dataStore, ApplicationName, transactionEmployee, usePayrollId);
            return employeeId;
        }

        private static string getEmpId(OpsContext opsContext, DataStoreClient dataStore, string ApplicationName, DbEmployee transactionEmployee, bool usePayrollId)
        {
            if (usePayrollId)
            {
                string payrollId = transactionEmployee.PayrollID;
                if (!string.IsNullOrEmpty(payrollId))
                {
                    return payrollId;
                }
            }

            return "1";
        }

        public static string GetEmployeeVatId(OpsContext opsContext, DataStoreClient dataStore, string ApplicationName)
        {
            int transEmployeeNumber = opsContext.TransEmployeeNumber;
            DbEmployee transactionEmployee = dataStore.ReadEmployeeByNum(transEmployeeNumber);
            bool usePayrollId = FiscalConfigUtil.UsePayrollId(opsContext, dataStore, ApplicationName);
            string empVatId = getEmpVatId(opsContext, dataStore, ApplicationName, usePayrollId, transactionEmployee);
            return empVatId;
        }

        private static string getEmpVatId(OpsContext opsContext, DataStoreClient dataStore, string ApplicationName, bool usePayrollId, DbEmployee transactionEmployee)
        {
            if (usePayrollId)
            {
                switch (FiscalConfigUtil.getTransactionType(opsContext, dataStore, ApplicationName))
                {
                    case TransactionEmployeeType.PID:
                        return transactionEmployee.PayrollID;
                    case TransactionEmployeeType.PIN:
                        return transactionEmployee.PIN;
                    default:
                        break;
                }
            }
            return dataStore.ReadExtensionDataValue("EMPLOYEE", "EmployeeVat", ((BaseDbKey<long>)(object)transactionEmployee.EmployeeID));
        }
    }
}
