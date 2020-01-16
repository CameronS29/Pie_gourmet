Imports Microsoft.VisualBasic
Imports System

Imports NVMS.WebStore

Public Class App
    Public Shared Configuration As WebConfiguration = New WebConfiguration()
End Class

' <summary>
' Configuration class used for the demos
' </summary>
Public Class WebConfiguration

    Public AccountEmail As String = "dmogolo@gmail.com"
    Public PayPalUrl As String = "https://api-3t.sandbox.paypal.com/nvp"

    ' <summary>
    ' CC Processor options
    ' AccessPoint - only one supported at this time
    ' </summary>
    Public CCProcessor As ccProcessors = ccProcessors.AuthorizeNet

    ' <summary>
    ' Determines whether Credit Cards are processed online.
    ' Requires that the other cc Keys are set.
    ' </summary>
    Public ccProcessCardsOnline As Boolean = True

    ' <summary>
    ' The MerchantId or Store ID
    ' </summary>
    Public CCMerchantId As String = "2sweetpies"

    ' <summary>
    ' Merchant Password if one is required
    ' </summary>
    Public CCMerchantPassword As String = "88gsb5EHbTC343LB"

    ' <summary>
    ' Determines how orders are processed either as a Sale or Pre-Auth
    ' </summary>
    Public CCProcessType As ccProcessTypes = ccProcessTypes.PreAuth

    ' <summary>
    ' HTTP Url to do remote processing. Depends on the gateway. See
    ' ccProcessing.chm help file for more details on urls and gateway 
    ' configuration settings.
    ' </summary>

    'Test URL: https://certification.authorize.net/gateway/transact.dll
    'Live URL: https://secure.authorize.net/gateway/transact.dll
    'Test Values: http://localhost/piegourmet/GetPostValues.aspx
    Public CCHostUrl As String = "https://secure.authorize.net/gateway/transact.dll"

    ' <summary>
    ' Refering URL - some services require that this is provided.
    ' </summary>
    Public CCReferingOrderUrl As String = "https://www.piegourmet.com/"

    ' <summary>
    ' The path to a Certificate file for credit card processing.
    ' </summary>
    Public CCCertificatePath As String = ""

    ' <summary>
    ' Url used to access PayPal
    ' </summary>
    Public CCPayPalUrl As String = "https://www.paypal.com/cgi-bin/webscr?"

    ' <summary>
    ' The Email Address for the account to distribute money to.
    ' </summary>
    Public CCPayPalEmail As String = "dmogolo@gmail.com"

    ' <summary>
    ' Determines wheter PayPal transactions can be auto-confirmed. 
    ' Useful to first check out PayPal transactions for fraud.
    ' </summary>
    Public CCPayPalAllowAutoConfirmation As Boolean = True

    ' <summary>
    ' The amount of time in seconds that we wait for completion of processing
    ' </summary>
    Public CCConnectionTimeout As Integer = 60

    ' <summary>
    ' Location of the Credit Card Log File. If empty no logging occurs.
    ' Requires a fully qualified OS path.
    ' </summary>
    'Development
    'Public CCLogFile As String = "D:\Project_Build\NVMS.Net\web\application\log\CCLog.txt"

    'Live
    Public CCLogFile As String = ConfigurationManager.AppSettings("LogFolder") & "\eft\AuthorizeLog.txt"

End Class
