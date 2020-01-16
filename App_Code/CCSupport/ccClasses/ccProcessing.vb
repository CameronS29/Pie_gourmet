Imports Microsoft.VisualBasic
Imports System
Imports System.IO
Imports System.Text
Imports System.Text.RegularExpressions
Imports System.Globalization
Imports NVMS.InternetTools

Namespace NVMS.WebStore
    ' <summary>
    ' The ccProcessing class provides a base class for provider processing of 
    ' requests. The class aims at providing a simple, common interface to Credit 
    ' Card Processing so it's easy to swap providers.
    ' 
    ' This class acts as an abstract class that requires subclassing and use of a
    ' specific provider implementation class. However, because the functionality is 
    ' so common amongst providers a single codebase can usually accomodate all requests.
    ' </summary>
    Public MustInherit Class ccProcessing

#Region "Logon Properties"
        ' <summary>
        ' The merchant Id or store name or other mechanism used to identify your account.
        ' </summary>
        Public Property MerchantId() As String
            Get
                Return _MerchantId
            End Get
            Set(ByVal value As String)
                _MerchantId = value
            End Set
        End Property
        Private _MerchantId As String = ""

        ' <summary>
        ' The merchant password for your merchant account. Not used in most cases.
        ' </summary>
        Public Property MerchantPassword() As String
            Get
                Return _MerchantPassword
            End Get
            Set(ByVal value As String)
                _MerchantPassword = value
            End Set
        End Property
        Private _MerchantPassword As String = ""

        ' <summary>
        ' The merchant username for your merchant account. Not used in most cases.
        ' </summary>
        Public Property MerchantUserName() As String
            Get
                Return _MerchantUserName
            End Get
            Set(ByVal value As String)
                _MerchantUserName = value
            End Set
        End Property
        Private _MerchantUserName As String = ""

#End Region

#Region "Processor Configuration"
        ' <summary>
        ' Reference to an wwHttp object. You can preseed this object after instantiation to 
        ' allow setting custom HTTP settings prior to calling ValidateCard().
        ' 
        ' Used only with providers that use direct POST operations to a Web Server
        ' including Authorize.NET, AccessPoint and BluePay
        ' </summary>
        Public Property Http() As wwHttp
            Get
                Return _Http
            End Get
            Set(ByVal value As wwHttp)
                _Http = value
            End Set
        End Property
        Private _Http As wwHttp = Nothing


        ' <summary>
        ' The link to hit on the server. Depending on the interface this can be a 
        ' URL or domainname or domainname:Port combination.
        ' 
        ' Applies only to providers that use HTTP POST operations directly.
        ' </summary>
        Public Property HttpLink() As String
            Get
                Return _HttpLink
            End Get
            Set(ByVal value As String)
                _HttpLink = value
            End Set
        End Property
        Private _HttpLink As String = ""


        ' <summary>
        ' Timeout in seconds for the connection against the remote processor
        ' </summary>
        Public Property Timeout() As Integer
            Get
                Return _Timeout
            End Get
            Set(ByVal value As Integer)
                _Timeout = value
            End Set
        End Property
        Private _Timeout As Integer = 50

        ' <summary>
        ' Optional flag that determines whether to send a test transaction
        ' Not supported for AccessPoint
        ' </summary>
        Public Property UseTestTransaction() As Boolean
            Get
                Return _UseTestTransaction
            End Get
            Set(ByVal value As Boolean)
                _UseTestTransaction = value
            End Set
        End Property
        Private _UseTestTransaction As Boolean = True

        ' <summary>
        ' Determines whether a Mod10Check is performed before sending the 
        ' credit card to the processor. Turn this off for testing so you
        ' can at least get to the provider.
        ' </summary>
        Public Property UseMod10Check() As Boolean
            Get
                Return _UseMod10Check
            End Get
            Set(ByVal value As Boolean)
                _UseMod10Check = value
            End Set
        End Property
        Private _UseMod10Check As Boolean = True

        ' <summary>
        ' Referring Url used with certain providers
        ' </summary>
        Public Property ReferringUrl() As String
            Get
                Return _ReferringUrl
            End Get
            Set(ByVal value As String)
                _ReferringUrl = value
            End Set
        End Property
        Private _ReferringUrl As String = ""


#End Region

#Region "Credit Card Information"
        ' <summary>
        ' The credit card number. Number can contain spaces and other markup 
        ' characters which are stripped for processing later.
        ' </summary>
        Public Property CreditCardNumber() As String
            Get
                Return _CreditCardNumber
            End Get
            Set(ByVal value As String)
                _CreditCardNumber = value
            End Set
        End Property
        Private _CreditCardNumber As String = ""

        ' <summary>
        ' The 3 or 4 letter digit that is on the back of the card
        ' </summary>
        Public Property SecurityCode() As String
            Get
                Return _SecurityCode
            End Get
            Set(ByVal value As String)
                _SecurityCode = value
            End Set
        End Property
        Private _SecurityCode As String = ""

        ' <summary>
        ' Full expiration date in the format 01/2003
        ' </summary>
        Public Property CreditCardExpiration() As String
            Get
                Return Me._CreditCardExpiration
            End Get
            Set(ByVal value As String)
                Dim Exp As String = value.Trim()
                Dim Split As String() = Exp.Split("/-.\,".ToCharArray(), 2)
                Me.CreditCardExpirationMonth = Split(0).PadLeft(2, "0"c)
                Me.CreditCardExpirationYear = Split(1)
                If Me.CreditCardExpirationYear.Length = 4 Then
                    Me.CreditCardExpirationYear = Me.CreditCardExpirationYear.Substring(2, 2)
                End If
                Me._CreditCardExpiration = Exp
            End Set
        End Property
        Private _CreditCardExpiration As String = ""

        ' <summary>
        ' Credit Card Expiration Month as a string (2 digits ie. 08)
        ' </summary>
        Public Property CreditCardExpirationMonth() As String
            Get
                Return _CreditCardExpirationMonth
            End Get
            Set(ByVal value As String)
                _CreditCardExpirationMonth = value
            End Set
        End Property
        Private _CreditCardExpirationMonth As String = ""

        ' <summary>
        ' Credit Card Expiration Year as a 4 digit string
        ' </summary>
        Public Property CreditCardExpirationYear() As String
            Get
                Return _CreditCardExpirationYear
            End Get
            Set(ByVal value As String)
                _CreditCardExpirationYear = value
            End Set
        End Property
        Private _CreditCardExpirationYear As String = ""

        ' <summary>
        ' Determines what type of transaction is being processed (Sale, Credit, PreAuth)
        ' </summary>
        Public Property ProcessType() As ccProcessTypes
            Get
                Return _ProcessType
            End Get
            Set(ByVal value As ccProcessTypes)
                _ProcessType = value
            End Set
        End Property
        Private _ProcessType As ccProcessTypes = ccProcessTypes.Sale

#End Region

#Region "Order Information"
        ' <summary>
        ' The amount of the order.
        ' </summary>
        Public Property OrderAmount() As Decimal
            Get
                Return _OrderAmount
            End Get
            Set(ByVal value As Decimal)
                _OrderAmount = value
            End Set
        End Property
        Private _OrderAmount As Decimal = 0

        ' <summary>
        ' The amount of Tax for this transaction
        ' </summary>
        Public Property TaxAmount() As Decimal
            Get
                Return _TaxAmount
            End Get
            Set(ByVal value As Decimal)
                _TaxAmount = value
            End Set
        End Property
        Private _TaxAmount As Decimal = 0

        ' <summary>
        ' The Order Id as a string. This is mainly for reference but should be unique.
        ' </summary>
        Public Property OrderId() As String
            Get
                Return _OrderId
            End Get
            Set(ByVal value As String)
                _OrderId = value
            End Set
        End Property
        Private _OrderId As String = ""

        ' <summary>
        ' Order Comment. Usually this comment shows up on the CC bill.
        ' </summary>
        Public Property Comment() As String
            Get
                Return _Comment
            End Get
            Set(ByVal value As String)
                _Comment = value
            End Set
        End Property
        Private _Comment As String = ""

#End Region

#Region "Customer/Billing Information"

        ' <summary>
        ' Billing Company
        ' </summary>
        Public Property CustomerID() As String
            Get
                Return _CustomerID
            End Get
            Set(ByVal value As String)
                _CustomerID = value
            End Set
        End Property
        Private _CustomerID As String = ""

        ' <summary>
        ' First name and last name on the card
        ' </summary>
        Public Property Name() As String
            Get
                If _Name = "" Then
                    Return (CStr(Firstname & " " & Lastname)).Trim()
                End If

                Return _Name
            End Get
            Set(ByVal value As String)
                _Name = value
            End Set
        End Property
        Private _Name As String = ""

        ' <summary>
        ' First Name of customer's name on the card. Can be used in lieu of Name 
        ' property. If Firstname and Lastname are used they get combined into a name 
        ' IF Name is blank.
        ' <seealso>Class ccProcessing</seealso>
        ' </summary>
        Public Property Firstname() As String
            Get
                Return _Firstname
            End Get
            Set(ByVal value As String)
                _Firstname = value
            End Set
        End Property
        Private _Firstname As String = ""



        ' <summary>
        ' Last Name of customer's name on the card. Can be used in lieu of Name 
        ' property. If Firstname and Lastname are used they get combined into a name 
        ' IF Name is blank.
        ' 
        ' AuthorizeNet requires both first and last names, while the other providers 
        ' only use a single Name property.
        ' <seealso>Class ccProcessing</seealso>
        ' </summary>
        Public Property Lastname() As String
            Get
                Return _Lastname
            End Get
            Set(ByVal value As String)
                _Lastname = value
            End Set
        End Property
        Private _Lastname As String = ""


        ' <summary>
        ' Billing Company
        ' </summary>
        Public Property Company() As String
            Get
                Return _Company
            End Get
            Set(ByVal value As String)
                _Company = value
            End Set
        End Property
        Private _Company As String = ""


        ' <summary>
        ' Billing Street Address.
        ' </summary>
        Public Property Address() As String
            Get
                Return _Address
            End Get
            Set(ByVal value As String)
                _Address = value
            End Set
        End Property
        Private _Address As String = ""

        ' <summary>
        ' Billing City
        ' </summary>
        Public Property City() As String
            Get
                Return _City
            End Get
            Set(ByVal value As String)
                _City = value
            End Set
        End Property
        Private _City As String = ""

        ' <summary>
        ' Billing State (2 letter code or empty for foreign)
        ' </summary>
        Public Property State() As String
            Get
                Return _State
            End Get
            Set(ByVal value As String)
                _State = value
            End Set
        End Property
        Private _State As String = ""

        ' <summary>
        ' Postal or Zip code
        ' </summary>
        Public Property Zip() As String
            Get
                Return _Zip
            End Get
            Set(ByVal value As String)
                _Zip = value
            End Set
        End Property
        Private _Zip As String = ""

        ' <summary>
        ' Two letter Country Id -  US, DE, CH etc.
        ' </summary>
        Public Property Country() As String
            Get
                Return _Country
            End Get
            Set(ByVal value As String)
                _Country = value
            End Set
        End Property
        Private _Country As String = ""

        ' <summary>
        ' Billing Phone Number 
        ' </summary>
        Public Property Phone() As String
            Get
                Return _Phone
            End Get
            Set(ByVal value As String)
                _Phone = value
            End Set
        End Property
        Private _Phone As String = ""

        ' <summary>
        ' Email address
        ' </summary>
        Public Property Email() As String
            Get
                Return _Email
            End Get
            Set(ByVal value As String)
                _Email = value
            End Set
        End Property
        Private _Email As String = ""

        ' <summary>
        ' Corp PO Number
        ' </summary>
        Public Property PONumber() As String
            Get
                Return _PONumber
            End Get
            Set(ByVal value As String)
                _PONumber = value
            End Set
        End Property
        Private _PONumber As String = ""

#End Region

#Region "Result Properties"
        ' <summary>
        ' The raw response from the Credit Card Processor Server.
        ' 
        ' You can use this result to manually parse out error codes
        ' and messages beyond the default parsing done by this class.
        ' </summary>
        Public Property RawProcessorResult() As String
            Get
                Return _RawProcessorResult
            End Get
            Set(ByVal value As String)
                _RawProcessorResult = value
            End Set
        End Property
        Private _RawProcessorResult As String = ""

        ' <summary>
        ' This is a string that contains the format that's sent to the
        ' processor. Not used with all providers, but this property can
        ' be used for debugging and seeing what exactly gets sent to the
        ' server.
        ' </summary>
        Public Property RawProcessorRequest() As String
            Get
                Return _RawProcessorRequest
            End Get
            Set(ByVal value As String)
                _RawProcessorRequest = value
            End Set
        End Property
        Private _RawProcessorRequest As String = ""

        ' <summary>
        ' The parsed short form response. APPROVED, DECLINED, FAILED, FRAUD
        ' </summary>
        Public Property ValidatedResult() As String
            Get
                Return _ValidatedResult
            End Get
            Set(ByVal value As String)
                _ValidatedResult = value
            End Set
        End Property
        Private _ValidatedResult As String = ""

        ' <summary>
        ' The parsed error message from the server if result is not APPROVED
        ' This message generally is a string regarding the failure like 'Invalid Card'
        ' 'AVS Error' etc. This info may or may not be appropriate for your customers
        ' to see - that's up to you.
        ' </summary>
        Public Property ValidatedMessage() As String
            Get
                Return _ValidatedMessage
            End Get
            Set(ByVal value As String)
                _ValidatedMessage = value
            End Set
        End Property
        Private _ValidatedMessage As String = ""


        ' <summary>
        ' The Transaction ID returned from the server. Use to match
        ' transactions against the gateway for reporting.
        ' </summary>
        Public Property TransactionId() As String
            Get
                Return _TransactionId
            End Get
            Set(ByVal value As String)
                _TransactionId = value
            End Set
        End Property
        Private _TransactionId As String = ""


        ' <summary>
        ' Authorization Code returned for Approved transactions from the gateway
        ' </summary>
        Public Property AuthorizationCode() As String
            Get
                Return _AuthorizationCode
            End Get
            Set(ByVal value As String)
                _AuthorizationCode = value
            End Set
        End Property
        Private _AuthorizationCode As String = ""

        ' <summary>
        ' The AVS Result code from the gateway if available
        ' </summary>
        Public Property AvsResultCode() As String
            Get
                Return _AvsResultCode
            End Get
            Set(ByVal value As String)
                _AvsResultCode = value
            End Set
        End Property
        Private _AvsResultCode As String = ""



        ' <summary>
        ' Used for Linkpoint only. Specifies the path to the certificate file
        ' </summary>
        Public CertificatePath As String = ""


#End Region

#Region "Errors and Debugging"

        ' <summary>
        ' Optional path to the log file used to write out request results.
        ' If this filename is blank no logging occurs. The filename specified
        ' here needs to be a fully qualified operating system path and the
        ' application has to be able to write to this path.
        ' </summary>
        Private TempFolder As String = ConfigurationManager.AppSettings("TempFolder")

        Public Property LogFile() As String
            Get
                Return _LogFile
            End Get
            Set(ByVal value As String)
                _LogFile = value
            End Set
        End Property
        Private _LogFile As String = TempFolder & "\CCLog.txt"



        ' <summary>
        ' Error flag set after a call to ValidateCard if an error occurs.
        ' </summary>
        Public Property [Error]() As Boolean
            Get
                Return _Error
            End Get
            Set(ByVal value As Boolean)
                _Error = value
            End Set
        End Property
        Private _Error As Boolean = False

        ' <summary>
        ' Error message if error flag is set or negative result is returned.
        ' Generally this value will contain the value of this.ValidatedResult
        ' for processor failures or more general API/HTTP failure messages.
        ' </summary>
        Public Property ErrorMessage() As String
            Get
                Return _ErrorMessage
            End Get
            Set(ByVal value As String)
                _ErrorMessage = value
            End Set
        End Property
        Private _ErrorMessage As String = ""


#End Region

        ' <summary>
        ' Base ValidateCard method that provides the core CreditCard checking. Should 
        ' always be called at the beginning of the subclassed overridden method.
        ' </summary>
        ' <returns>bool</returns>
        Public Overridable Function ValidateCard() As Boolean
            If Me.UseMod10Check AndAlso (Not Mod10Check(Me.CreditCardNumber)) Then
                Me.ErrorMessage = "Invalid Credit Card Number"
                Me.ValidatedMessage = Me.ErrorMessage
                Me.ValidatedResult = "DECLINED"
                Me.LogTransaction()
                Return False
            End If

            Return True
        End Function

        ' <summary>
        ' Logs the information of the current request into the log file specified.
        ' If the log file is empty no logging occurs.
        ' </summary>
        ' <param name="lcLogString"></param>
        Protected Overridable Sub LogTransaction()
            If Me.LogFile <> "" Then
                Dim CardNo As String = Me.CreditCardNumber
                Dim Result As String = Me.RawProcessorResult

                If Me.CreditCardNumber.Length > 10 Then
                    CardNo = CardNo.Substring(0, 10)
                End If

                Dim binLogString As Byte() = Encoding.Default.GetBytes(DateTime.Now.ToString() & " - " & Me.Name & " [" & Me.CustomerID & "] - " & CardNo & " - Result: " & Result & " - Result Msg: " & Me.ValidatedResult & " - " & Me.ValidatedMessage & " - " & Me.OrderAmount.ToString(CultureInfo.InstalledUICulture.NumberFormat) & Constants.vbCrLf)

                SyncLock Me
                    Try
                        Dim loFile As FileStream = New FileStream(LogFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write)
                        loFile.Seek(0, SeekOrigin.End)
                        loFile.Write(binLogString, 0, binLogString.Length)
                        loFile.Close()
                    Catch

                    End Try
                End SyncLock
            End If
        End Sub

        Protected Overridable Sub LogTransactionRaw()
            If Me.LogFile <> "" Then
                Dim CardNo As String = Me.CreditCardNumber
                Dim Result As String = Me.RawProcessorResult

                Dim binLogString As Byte() = Encoding.Default.GetBytes(DateTime.Now.ToString() & " - " & Me.Name & " [" & Me.CustomerID & "] - " & CardNo & " - ResultMsg: " & Result & " - " & Me.OrderAmount.ToString(CultureInfo.InstalledUICulture.NumberFormat) & Constants.vbCrLf)

                SyncLock Me
                    Try
                        Dim loFile As FileStream = New FileStream(LogFile, FileMode.OpenOrCreate, FileAccess.Write, FileShare.Write)
                        loFile.Seek(0, SeekOrigin.End)
                        loFile.Write(binLogString, 0, binLogString.Length)
                        loFile.Close()
                    Catch

                    End Try
                End SyncLock
            End If
        End Sub

        ' <summary>
        ' Returns a string for a single AVS code value
        ' Supported codes:
        ' ANSUWXYZER_
        ' </summary>
        ' <param name="AvsCode"></param>
        ' <returns></returns>
        Public Function AvsCodeToString(ByVal AvsCode As String) As String
            If AvsCode = "A" Then
                Return "Street Address Matched, Zip not Matched"
            End If
            If AvsCode = "N" Then
                Return "No AVS Match"
            End If
            If AvsCode = "U" Then
                Return "AVS not supported for this card type"
            End If
            If AvsCode = "W" Then
                Return "Zip 9 matched, no street match"
            End If
            If AvsCode = "X" Then
                Return "Zip 9 matched, street matched"
            End If
            If AvsCode = "Y" Then
                Return "Zip 5 matched, street matched"
            End If
            If AvsCode = "Z" Then
                Return "Zip 5 matched, street not matched"
            End If
            If AvsCode = "E" Then
                Return "Not eligible for AVS"
            End If
            If AvsCode = "R" Then
                Return "System unavailable"
            End If
            If AvsCode = "_" Then
                Return "AVS supported on this network or transaction type"
            End If

            Return ""
        End Function

        ' <summary>
        ' Determines whether given string passes standard Mod10 check.
        ' </summary>
        ' <param name="validString">String to be validated</param>
        ' <returns>True if valid, otherwise false</returns>
        Public Shared Function Mod10Check(ByVal StringToValidate As String) As Boolean
            Dim trimString As String = StringToValidate.Trim()
            Dim lastChar As Char = trimString.Chars(trimString.Length - 1)

            Dim checkSumChar As Char = Mod10Worker(trimString.Substring(0, trimString.Length - 1), False)
            Return (lastChar = checkSumChar)
        End Function

        ' <summary>
        ' Worker Mod10 check method that figures out the Mod10 checksum value for a string
        ' </summary>
        ' <param name="chkString">String to be validated</param>
        ' <param name="startLeft">Specifies if check starts from left</param>
        ' <returns>Checksum character</returns>
        Private Shared Function Mod10Worker(ByVal chkString As String, ByVal startLeft As Boolean) As Char
            ' Remove any non-alphanumeric characters (The list can be expanded by adding characters
            ' to RegEx.Replace pattern parameter's character set)
            Dim chkValid As String = chkString
            chkValid = Regex.Replace(chkValid, "[\s-\.]*", "").ToUpper()

            ' Calculate the MOD 10 check digit
            Dim Mod10 As Integer = 0
            Dim Digit As Integer
            Dim curChar As Char

            For Pos As Integer = 0 To chkValid.Length - 1
                ' Take each char starting from the right unless startLeft is true
                If startLeft Then
                    Digit = Pos
                Else
                    Digit = chkValid.Length - Pos - 1
                End If

                curChar = chkValid.Chars(Digit)

                ' If the character is a digit, take its numeric value.
                If System.Char.IsDigit(curChar) Then
                    Digit = CInt(Fix(System.Char.GetNumericValue(curChar)))
                Else
                    ' Otherwise, take the base16 value of the letter.
                    ' NOTE: The .... is a place holder to force the / to equal 15.
                    ' The USPS does not assign a value for a period. Periods were removed from
                    ' string at beginning of the function
                    Dim base16String As String = "ABCDEFGHIJKLMNOPQRSTUVWXYZ..../"
                    Dim foundPos As Integer = base16String.IndexOf(curChar) + 1
                    If foundPos >= 0 Then
                        Digit = (foundPos Mod 16)
                    Else
                        Digit = (0)
                    End If
                End If

                ' Multiply by 2 if the position is odd
                If (Pos + 1) Mod 2 = 0 Then
                    Digit = Digit * (1)
                Else
                    Digit = Digit * (2)
                End If

                ' If the result is larger than 10, the two digits have to be added. This can be done 
                ' by adding the results of the integer division by 10 and then mod by 10.
                Digit = (Digit \ 10) + (Digit Mod 10)

                ' If the result is 10 (which occurs when N is an odd position), add the two digits together again
                ' (this becomes 1).
                If Digit = 10 Then
                    Digit = (1)
                Else
                    Digit = (Digit)
                End If

                ' Sum all the digits
                Mod10 = Mod10 + Digit
            Next Pos

            ' Subtract the MOD 10 from 10
            Mod10 = 10 - (Mod10 Mod 10)

            ' If the result is 10, then the check digit is 0.
            ' Else, it is the result. Return it as a character
            If Mod10 = 10 Then
                Mod10 = (0)
            Else
                Mod10 = (Mod10)
            End If

            Return Mod10.ToString().Chars(0)
        End Function

        ' <summary>
        ' Error setting method that sets the Error flag and message.
        ' </summary>
        ' <param name="lcErrorMessage"></param>
        Protected Sub SetError(ByVal ErrorMessage As String)
            If ErrorMessage Is Nothing OrElse ErrorMessage.Length = 0 Then
                Me.ErrorMessage = ""
                Me.Error = False
                Return
            End If

            Me.ErrorMessage = ErrorMessage
            Me.Error = True
        End Sub
    End Class

    ' <summary>
    ' Credit Card Processors available
    ' </summary>
    Public Enum ccProcessors
        AuthorizeNet
        AccessPoint
        PayFlowPro
        LinkPoint
        BluePay
        ECLinx
    End Enum

    Public Enum ccProcessTypes
        Sale
        Credit
        PreAuth
        AuthCapture
        AUTH_ONLY
    End Enum

End Namespace
