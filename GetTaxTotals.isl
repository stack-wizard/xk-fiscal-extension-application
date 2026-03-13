NetImport from "C:\Micros\Simphony\WebServer\wwwroot\EGateway\Handlers\ExtensionApplications\Mikos.XK.Fiscal\Mikos.XK.Fiscal.dll"

var Tax01Name           : A5 = "18.00"
var Tax01Gross          : $12 
var Tax01Net            : $12 
var Tax01Vat            : $12 
var Tax02Name           : A5 = "0.00"
var Tax02Gross          : $12
var Tax02Net            : $12
var Tax02Vat            : $12
var Tax03Name           : A5 = ""
var Tax03Gross          : $12 = 0
var Tax03Net            : $12 = 0
var Tax03Vat            : $12 = 0
var Tax04Name           : A5 = ""
var Tax04Gross          : $12 = 0    
var Tax04Net            : $12 = 0    
var Tax04Vat            : $12 = 0 
var Tax05Name           : A5 = ""
var Tax05Gross          : $12 = 0   // Currently not used
var Tax05Net            : $12 = 0   // Currently not used
var Tax05Vat            : $12 = 0   // Currently not used
var Tax06Name           : A5 = ""
var Tax06Gross          : $12 = 0   // Currently not used
var Tax06Net            : $12 = 0   // Currently not used
var Tax06Vat            : $12 = 0   // Currently not used
var Tax07Name           : A5 = ""
var Tax07Gross          : $12 = 0   // Currently not used
var Tax07Net            : $12 = 0   // Currently not used
var Tax07Vat            : $12 = 0   // Currently not used
var Tax08Name           : A5 = ""
var Tax08Gross          : $12 = 0   // Currently not used
var Tax08Net            : $12 = 0   // Currently not used
var Tax08Vat            : $12 = 0   // Currently not used

event inq : GetTaxTotals
    SetSignOnLeft
    call TaxCalc
    Mikos.XK.Fiscal.Util.TaxTotalsUtil.ReceiveTaxTotals(Tax01Name, Tax01Net, Tax01Vat, \
    Tax02Name, Tax02Net, Tax02Vat)
endevent

sub TaxCalc
    Tax01Vat = 0
    Tax01Gross = 0
    Tax01Net = 0
    Tax02Vat = 0
    Tax02Gross = 0
    Tax02Net = 0

    
    if @TXBL[ 1 ] <> 0 
        Tax01Vat = Tax01Vat + @TAXVAT[ 1 ]
        Tax01Gross = Tax01Gross + @TXBL[ 1 ]
        Tax01Net = Tax01Gross - @TAXVAT[ 1 ]
    endif

    if @TXBL[ 2 ] <> 0 
        Tax02Vat = Tax02Vat + @TAXVAT[ 1 ]
        Tax02Gross = Tax02Gross + @TXBL[ 1 ]
        Tax02Net = Tax02Gross - @TAXVAT[ 1 ]
    endif

endsub