using System.Collections.Generic;

namespace Mikos.XK.Fiscal.Dtos
{
    public class FiscalRequestData
    {
        public DepositsInfo DepositsInfo { get; set; }
        public DocumentInfo DocumentInfo { get; set; }
        public AdditionalInfo AdditionalInfo { get; set; }
        public UserDefinedFields UserDefinedFields { get; set; }
        public FiscalTerminalInfo FiscalTerminalInfo { get; set; }
        public FolioInfo FolioInfo { get; set; }
        public HotelInfo HotelInfo { get; set; }
        public ReservationInfo ReservationInfo { get; set; }
        public FiscalFolioUserInfo FiscalFolioUserInfo { get; set; }
        public CollectingAgentPropertyInfo CollectingAgentPropertyInfo { get; set; }
        public VersionInfo VersionInfo { get; set; }
        public FiscalPartnerResponse FiscalPartnerResponse { get; set; }
    }


    public class AccompanyingGuest
    {
        public List<ExternalRefInfo> ExternalRefInfo { get; set; }
        public UserDefinedFields UserDefinedFields { get; set; }
        public IdentificationInfos IdentificationInfos { get; set; }
        public KeywordInfos KeywordInfos { get; set; }
        public Addresses Address { get; set; }
        public Phone Phone { get; set; }
        public WebPage WebPage { get; set; }
    }

    public class AccompanyingGuestInfo
    {
        public List<AccompanyingGuest> AccompanyingGuest { get; set; }
    }

    public class AdditionalInfo
    {
        public BeforeSettlement BeforeSettlement { get; set; }
        public ProfileOptions ProfileOptions { get; set; }
        public MappedValues MappedValues { get; set; }
        public ReservationOptions ReservationOptions { get; set; }
    }

    public class Addresses
    {
        public string Address { get; set; }
        public string Address1 { get; set; }

        public string AddresseeCountryDesc { get; set; }

        public string City { get; set; }

        public string Country { get; set; }

        public string IsoCode { get; set; }

        public string PostalCode { get; set; }
        public bool Primary { get; set; }
        public string Type { get; set; }
    }

    public class Amount
    {
    }

    public class AmountUDFsType
    {
        public List<Amount> Amounts { get; set; }
    }

    public class ArrangementInfo
    {
    }

    public class Article
    {
    }

    public class Articles
    {
        public List<Article> Article { get; set; }
    }

    public class AssociatedFiscalTerminalInfo
    {
    }

    public class AssociatedFolioInfo
    {
        public AssociatedFiscalTerminalInfo AssociatedFiscalTerminalInfo { get; set; }
        public CollectingAgentTaxes CollectingAgentTaxes { get; set; }
    }

    public class BeforeSettlement
    {
        public List<NV> NV { get; set; }
    }

    public class CharacterUDF
    {
        public List<UDF> UDF { get; set; }
    }

    public class CollectingAgentPropertyInfo
    {
        public TaxPercents TaxPercents { get; set; }
        public TriggerAmounts TriggerAmounts { get; set; }
    }

    public class CollectingAgentTaxes
    {
        public AmountUDFsType AmountUDFsType { get; set; }
    }

    public class DateUDF
    {
        public List<UDF> UDF { get; set; }
    }

    public class DepositInfo
    {
        public FolioHeaderInfo FolioHeaderInfo { get; set; }
        public List<Posting> Postings { get; set; }
        public List<RevenueBucketInfo> RevenueBucketInfo { get; set; }
        public List<TrxInfo> TrxInfo { get; set; }
    }

    public class DepositsInfo
    {
        public List<DepositInfo> DepositInfo { get; set; }
    }

    public class DocumentInfo
    {
        public LastSupportingDocumentInfo LastSupportingDocumentInfo { get; set; }
        public string HotelCode { get; set; }
        public string BillNo { get; set; }
        public string FolioType { get; set; }
        public string TerminalId { get; set; }
        public string ProgramName { get; set; }
        public string FiscalFolioId { get; set; }
        public string OperaFiscalBillNo { get; set; }
        public string Application { get; set; }
        public string PropertyTaxNumber { get; set; }
        public string BankName { get; set; }
        public string BankCode { get; set; }
        public string BankIdType { get; set; }
        public string BankIdCode { get; set; }
        public string BusinessDate { get; set; }
        public string BusinessDateTime { get; set; }
        public string CountryCode { get; set; }
        public string CountryName { get; set; }
        public string Command { get; set; }
        public string FiscalTimeoutPeriod { get; set; }
    }

    public class ExchangeRateInfo
    {
        public string CurrencyCode { get; set; }
    }

    public class ExchangeRates
    {
        public List<ExchangeRateInfo> ExchangeRateInfo { get; set; }
    }

    public class ExternalRefInfo
    {
    }

    public class FiscalFolioUserInfo
    {
        public string AppUser { get; set; }
        public string AppUserId { get; set; }
        public string EmployeeNumber { get; set; }
        public string CashierId { get; set; }
    }

    public class FiscalPartnerResponse
    {
        public FiscalResponse FiscalResponse { get; set; }
    }

    public class FiscalResponse
    {
        public List<NV> NV { get; set; }
    }

    public class FiscalTerminalInfo
    {
        public string TerminalID { get; set; }
    }

    public class FolioHeaderInfo
    {
        public AssociatedFiscalTerminalInfo AssociatedFiscalTerminalInfo { get; set; }
        public string BillGenerationDate { get; set; }
        public string FolioType { get; set; }
        public bool CreditBill { get; set; }
        public string FolioNo { get; set; }
        public string BillNo { get; set; }
        public string InvoiceCurrencyCode { get; set; }
        public string InvoiceCurrencyRate { get; set; }
        public string Window { get; set; }
        public string CashierNumber { get; set; }
        public string FiscalFolioStatus { get; set; }
        public string LocalBillGenerationDate { get; set; }
        public string FolioTypeUniqueCode { get; set; }
        public CollectingAgentTaxes CollectingAgentTaxes { get; set; }
        public AssociatedFolioInfo AssociatedFolioInfo { get; set; }
    }

    public class FolioInfo
    {
        public List<ArrangementInfo> ArrangementInfo { get; set; }
        public FolioHeaderInfo FolioHeaderInfo { get; set; }
        public PayeeInfo PayeeInfo { get; set; }
        public List<Posting> Postings { get; set; }
        public List<RevenueBucketInfo> RevenueBucketInfo { get; set; }
        public TotalInfo TotalInfo { get; set; }
        public List<TrxInfo> TrxInfo { get; set; }
        public List<PosChequeInfo> PosChequeInfo { get; set; }
    }

    public class Generate
    {
        public string Currency { get; set; }
        public double ExchangeRate { get; set; }
        public string LocalTrxDateTime { get; set; }
        public decimal Quantity { get; set; }
        public bool TaxInclusive { get; set; }
        public double TaxRate { get; set; }
        public string TrxCode { get; set; }
        public string TrxDate { get; set; }
        public string TrxDateTime { get; set; }
        public int TrxNo { get; set; }
        public string TrxType { get; set; }
        public decimal UnitPrice { get; set; }
        public int FinDmlSeqNo { get; set; }
        public decimal NetAmount { get; set; }
        public string Reference { get; set; }
        public int TranActionId { get; set; }
        public int TrxNoAddedBy { get; set; }
    }

    public class Generates
    {
        public List<Generate> Generate { get; set; }
    }

    public class GuestInfo
    {
        public List<ExternalRefInfo> ExternalRefInfo { get; set; }
        public UserDefinedFields UserDefinedFields { get; set; }
        public IdentificationInfos IdentificationInfos { get; set; }
        public KeywordInfos KeywordInfos { get; set; }
        public Addresses Address { get; set; }
        public Phone Phone { get; set; }
        public WebPage WebPage { get; set; }
    }

    public class HotelInfo
    {
        public string HotelCode { get; set; }
        public string HotelName { get; set; }
        public string LegalOwner { get; set; }
        public string LocalCurrency { get; set; }
        public string Decimals { get; set; }
        public string TimeZoneRegion { get; set; }
        public string PhoneNo { get; set; }
        public string Email { get; set; }
        public string WebPage { get; set; }
        public string PropertyDateTime { get; set; }
        public string BusinessPremiseId1 { get; set; }
        public string BusinessPremiseId2 { get; set; }
        public Addresses Address { get; set; }
        public ExchangeRates ExchangeRates { get; set; }
    }

    public class IdentificationInfo
    {
        public string IdType { get; set; }
        public string IdNumber { get; set; }
        public bool Primary { get; set; }
    }

    public class IdentificationInfos
    {
        public List<IdentificationInfo> IdentificationInfo { get; set; }
    }

    public class KeywordInfo
    {
    }

    public class KeywordInfos
    {
        public List<KeywordInfo> KeywordInfo { get; set; }
    }

    public class LastSupportingDocumentInfo
    {
    }

    public class LinkedProfile
    {
        public WebPage WebPage { get; set; }
    }

    public class LinkedProfiles
    {
        public List<LinkedProfile> LinkedProfile { get; set; }
    }

    public class MappedValue
    {
        public string Code { get; set; }
        public List<NV> NV { get; set; }
    }

    public class MappedValues
    {
        public List<MappingType> MappingType { get; set; }
    }

    public class MappingType
    {
        public string Type { get; set; }
        public List<MappedValue> MappedValue { get; set; }
    }

    public class NumericUDF
    {
        public List<UDF> UDF { get; set; }
    }

    public class NV
    {
        public string Name { get; set; }
        public string Value { get; set; }
        public string BusinessDate { get; set; }
        public int ResponseSeqNo { get; set; }
    }

    public class OPERA5
    {
        public string OPERA5Version { get; set; }
        public int Major { get; set; }
        public int Minor { get; set; }
        public int Patch { get; set; }
        public int Patchset { get; set; }
    }

    public class OPERACloud
    {
    }

    public class PayeeInfo
    {
        public int? NameId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string DateOfBirth { get; set; }
        public string Passport { get; set; }
        public string Tax1No { get; set; }
        public string Tax2No { get; set; }
        public string PaymentDueDate { get; set; }
        public string Nationality { get; set; }
        public string Language { get; set; }
        public string NameType { get; set; }
        public List<ExternalRefInfo> ExternalRefInfo { get; set; }
        public UserDefinedFields UserDefinedFields { get; set; }
        public IdentificationInfos IdentificationInfos { get; set; }
        public KeywordInfos KeywordInfos { get; set; }
        public Addresses Address { get; set; }
        public Phone Phone { get; set; }
        public WebPage WebPage { get; set; }
    }

    public class Phone
    {
    }

    public class PosChequeInfo
    {
        public List<object> ChequeDetails { get; set; }
        public List<object> ChequeDate { get; set; }
    }

    public class Posting
    {
        public string TrxNo { get; set; }
        public string TrxCode { get; set; }
        public string TrxDate { get; set; }
        public string TrxType { get; set; }
        public decimal UnitPrice { get; set; }
        public double Quantity { get; set; }
        public string Currency { get; set; }
        public bool TaxInclusive { get; set; }
        public double ExchangeRate { get; set; }
        public string TrxDateTime { get; set; }
        public string LocalTrxDateTime { get; set; }
        public decimal NetAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal GuestAccountDebit { get; set; }
        public decimal GuestAccountCredit { get; set; }
        public string ArrangementCode { get; set; }
        public int TranActionId { get; set; }
        public int FinDmlSeqNo { get; set; }
        public string Reference { get; set; }
        public Generates Generates { get; set; }
    }

    public class ProfileOptions
    {
        public List<NV> NV { get; set; }
    }

    public class ReservationInfo
    {
        public string ConfirmationNo { get; set; }
        public string ResvNameID { get; set; }
        public string ArrivalDate { get; set; }
        public string ArrivalTime { get; set; }
        public string NumberOfNights { get; set; }
        public string DepartureDate { get; set; }
        public string NumAdults { get; set; }
        public string NumChilds { get; set; }
        public GuestInfo GuestInfo { get; set; }
        public string ChildAgeBucket1 { get; set; }
        public string ChildAgeBucket2 { get; set; }
        public string ChildAgeBucket3 { get; set; }
        public decimal RoomRate { get; set; }
        public string RatePlanCode { get; set; }
        public string RoomNumber { get; set; }
        public string RoomClass { get; set; }
        public string RoomType { get; set; }
        public string NumberOfRooms { get; set; }
        public string Guarantee { get; set; }
        public string MarketCode { get; set; }
        public string ResStatus { get; set; }
        public UserDefinedFields UserDefinedFields { get; set; }
        public string SourceCode { get; set; }
        public string SourceGroup { get; set; }
        public List<ExternalRefInfo> ExternalRefInfo { get; set; }
        public LinkedProfiles LinkedProfiles { get; set; }
        public AccompanyingGuestInfo AccompanyingGuestInfo { get; set; }
    }

    public class ReservationOptions
    {
        public List<NV> NV { get; set; }
    }

    public class RevenueBucketInfo
    {
        public string BucketCode { get; set; }
        public string BucketType { get; set; }
        public string BucketValue { get; set; }
        public string Description { get; set; }
        public decimal BucketCodeTotalGross { get; set; }
        public List<string> TrxCode { get; set; }
    }


    public class Tax
    {
        public Tax(int taxNum, decimal valueTotal, decimal totalNetAmount, string taxRateString, string amount)
        {
            Name = taxNum.ToString();
            Value = valueTotal;
            NetAmount = totalNetAmount;
            Percent = taxRateString;
            Amount = amount;
        }
        public string Name { get; set; }
        public decimal Value { get; set; }
        public decimal NetAmount { get; set; }
        public string Percent { get; set; }
        public string Amount { get; set; }
    }

    public class Taxes
    {
        public List<Tax> Tax { get; set; }
    }

    public class TaxPercents
    {
        public List<UDF> UDF { get; set; }
    }

    public class TotalInfo
    {
        public decimal NetAmount { get; set; }
        public decimal GrossAmount { get; set; }
        public decimal NonTaxableAmount { get; set; }
        public decimal PaidOut { get; set; }
        public Taxes Taxes { get; set; }
    }

    public class TranslatedDescription
    {
    }

    public class TranslatedDescriptions
    {
        public List<TranslatedDescription> TranslatedDescription { get; set; }
    }

    public class TriggerAmounts
    {
        public List<Amount> Amounts { get; set; }
    }

    public class TrxInfo
    {
        public string HotelCode { get; set; }
        public string Group { get; set; }
        public string SubGroup { get; set; }
        public string Code { get; set; }
        public string TrxType { get; set; }
        public string Description { get; set; }
        public string TrxCodeType { get; set; }
        public Articles Articles { get; set; }
        public TranslatedDescriptions TranslatedDescriptions { get; set; }
    }

    public class UDF
    {
        public string Name { get; set; }
        public string Value { get; set; }
    }

    public class UserDefinedFields
    {
        public List<CharacterUDF> CharacterUDFs { get; set; }
        public List<NumericUDF> NumericUDFs { get; set; }
        public List<DateUDF> DateUDFs { get; set; }
    }

    public class VersionInfo
    {
        public string PayloadVersion { get; set; }
        public string ApplicationPath { get; set; }
        public OPERA5 OPERA5 { get; set; }
        public OPERACloud OPERACloud { get; set; }
    }

    public class WebPage
    {
    }
}
