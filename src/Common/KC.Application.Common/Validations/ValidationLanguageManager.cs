using System;
using System.Collections.Generic;
using System.Reflection;
using FluentValidation;
using FluentValidation.Results;

namespace KC.Application.Common.Validations
{
    public static class ValidationCodes
    {
        [Message("At least one of the following fields are required: Email, Mobile Phone, or Home Phone")]
        public const string ContactInfoRequired = "ContactInfoRequired";

        [Message("At least one of the following fields are required: Mobile Phone, or Home Phone")]
        public const string MobileOrHomePhoneRequired = "MobileOrHomePhoneRequired";

        [Message("A customer with this email already exists.")]
        public const string CustomerAlreadyExists = "CustomerAlreadyExists";

        [Message("The specified unitId does not exist on this deal.")]
        public const string InvalidDealUnitId = "InvalidDealUnitId";

        public const string DealAlreadyExported = "DealAlreadyExported";

        [Message("A unit with the specified NumOrder already exists for this deal.")]
        public const string DuplicateUnitNumOrder = "DuplicateUnitNumOrder";

        [Message("A trade-in with the specified NumOrder already exists for this deal.")]
        public const string DuplicateTradeInNumOrder = "DuplicateTradeInNumOrder";

        [Message("'{PropertyName}' is not a valid email address.")]
        public const string EmailAddressValidator = "EmailAddressValidator";

        [Message("A user with this email already exists.")]
        public const string EmailAlreadyExists = "EmailAlreadyExists";

        [Message("{PropertyName}' has a range of values which does not include '{PropertyValue}'.")]
        public const string EnumValidator = "EnumValidator";

        [Message("There was an error generating contract(s).")]
        public const string ErrorGeneratingContract = "ErrorGeneratingContract";

        [Message("'{PropertyName}' should be in the format 99-9999999.")]
        public const string FederalTaxIdValidator = "FederalTaxIdValidator";

        [Message("'{PropertyName}' must be greater than '{ComparisonValue}'.")]
        public const string GreaterThanValidator = "GreaterThanValidator";

        [Message("'{PropertyName}' must be greater than or equal to '{ComparisonValue}'.")]
        public const string GreaterThanOrEqualValidator = "GreaterThanOrEqualValidator";

        [Message("This deal has been finalized and can no longer be imported.")]
        public const string ImportDealNotFound = "ImportDealNotFound";

        [Message("This deal was not imported from a DMS/CRM and cannot be updated.")]
        public const string ImportInfoMissing = "ImportInfoMissing";

        [Message("'{PropertyName}' must be between {From} and {To}.")]
        public const string InclusiveBetweenValidator = "InclusiveBetweenValidator";

        [Message("The specified '{PropertyName}' does not exist.")]
        public const string InvalidRoleId = "InvalidRoleId";

        [Message("This invitation has already been accepted. Please contact your system administrator for further assistance.")]
        public const string InvitationAlreadyAccepted = "InvitationAlreadyAccepted";

        [Message("This invitation has been deleted. Please contact your system administrator or the original sender for a new invitation link.")]
        public const string InvitationNotFound = "InvitationNotFound";

        [Message("The length of '{PropertyName}' must be {MaxLength} characters or fewer. You entered {TotalLength} characters.")]
        public const string MaximumLengthValidator = "MaximumLengthValidator";

        [Message("'{PropertyName}' is required.")]
        public const string NotEmptyValidator = "NotEmptyValidator";

        [Message("'{PropertyName}' must not be equal to '{ComparisonValue}'.")]
        public const string NotEqualValidator = "NotEqualValidator";

        [Message("'{PropertyName}' is required.")]
        public const string NotNullValidator = "NotNullValidator";

        [Message("Dealer with specified '{PropertyName}' could not be found.")]
        public const string OrgDoesNotExist = "OrgDoesNotExist";

        [Message("This email address already exists. Please use a different email address OR select the existing email from the auto-complete suggestions.")]
        public const string OrgUserAlreadyExists = "OrgUserAlreadyExists";

        [Message("'{PropertyName}' contains characters not normally used in a person's name.")]
        public const string PersonNameValidator = "PersonNameValidator";

        [Message("'{PropertyName}' should be in the format (999) 999-9999.")]
        public const string PhoneNumberValidator = "PhoneNumberValidator";

        [Message("'{PropertyName}' is not in the correct format.")]
        public const string RegularExpressionValidator = "RegularExpressionValidator";

        [Message("'{PropertyName}' should be in the format 999-99-9999.")]
        public const string SocialSecurityValidator = "SocialSecurityValidator";

        [Message("Either address1, city, and state or zipCode is required.")]
        public const string TaxRateRequestAddressMissing = "TaxRateRequestAddressMissing";

        [Message("Please read and agree to the terms of use.")]
        public const string TermsOfUseAcceptedValidation = "TermsOfUseAcceptedValidation";

        [Message("The specified '{PropertyName}' already exists in the system.")]
        public const string UniqueOrgCodeValidator = "UniqueOrgCodeValidator";

        [Message("'{PropertyName}' is not a valid URL.")]
        public const string UrlValidator = "UrlValidator";

        [Message("You have already completed the dealer enrollment.")]
        public const string UserAlreadyEnrolled = "UserAlreadyEnrolled";

        [Message("'{PropertyName}' should be in the format 99999 or 99999-9999.")]
        public const string ZipCodeValidator = "ZipCodeValidator";

        [Message("'{PropertyName}' does not exist.")]
        public const string IdDoesNotExist = "IdDoesNotExist";

        [Message("Either mileage or hours must be specified.")]
        public const string MileageHoursRequired = "MileageHoursRequired";

        [Message("The borrower must be at least 18 years of age.")]
        public const string BorrowerAge = "BorrowerAge";

        [Message("Unit is required for the product.")]
        public const string UnitRequiredForProduct = "UnitRequiredForProduct";

        [Message("Only one term must be selected.")]
        public const string SelectedTermsCount = "SelectedTermsCount";

        [Message("Only one column must be selected.")]
        public const string SelectedColumnsCount = "SelectedColumnsCount";

        [Message("Only one column can be custom.")]
        public const string CustomColumnsCount = "CustomColumnsCount";

        [Message("{PropertyName}' must be a Base-64 string.")]
        public const string Base64String = "Base64String";

        [Message("A lead with the same information already exists. Do you still want to submit this form and create a new lead?")]
        public const string LeadAlreadyExists = "LeadAlreadyExists";

        [Message("This feature is only available with FULL integration.")]
        public const string FullIntegrationRequired = "FullIntegrationRequired";

        [Message("Missing information on boat engine.")]
        public const string MarineEngineRequired = "MarineEngineRequired";

        [Message("A role with the same name already exists.")]
        public const string RoleAlreadyExists = "RoleAlreadyExists";

        public const string NumberOfReferencesValidator = "NumberOfReferencesValidator";

        [Message("This lender has already been added to the deal.")]
        public const string LenderAlreadyExists = "LenderAlreadyExists";

        [Message("Duplicate entry found for same fee type, name and deal type.")]
        public const string FeesDuplicateEntry = "FeesDuplicateEntry";

        [Message("A business with the same information already exists")]
        public const string BusinessAlreadyExists = "BusinessAlreadyExists";

        [Message("Invalid OTP.")]
        public const string InvalidOTP = "InvalidOTP";

        [Message("OTP Required.")]
        public const string OTPRequired = "OTPRequired";

        [Message("Duplicate tax profile entry found based on name, deal type and jurisdiction type.")]
        public const string TaxProfilesDuplicateEntry = "TaxProfilesDuplicateEntry";

        [Message("Email address is not allowed as per your organization policy.")]
        public const string ApprovedEmailDomains = "ApprovedEmailDomains";

        [Message("A Quote can only be accepted or declined if it is in an active state and has a unit and worksheet entered.")]
        public const string QuoteStateNotActive = "QuoteStateNotActive";

        [Message("An error has occured while retrieving credit report.")]
        public const string CreditReportError = "CreditReportError";

        [Message("Invalid Doc id or Doc id does not belong with this org.")]
        public const string InvalidDocIds = "InvalidDocIds";

        [Message("The provided phone number is formatted properly, but is an invalid phone number.")]
        public const string AzureAdPhoneError = "AzureAdPhoneError";

        [Message("Another Deal/Quote already exists for Stock and VIN.")]
        public const string DuplicateDealUnit = "DuplicateDealUnit";

        [Message("'{PropertyName}' contains characters not normally used in a '{PropertyName}'.")]
        public const string AlphabeticValidator = "AlphabeticValidator";

        [Message("There was an unexpected error enabling the provider. Please try again later.")]
        public const string ProviderEnrollmentFailure = "ProviderEnrollmentFailure";

        [Message("Duplicate check list entry found based on name, deal type, stage, customer type, is trade in, file upload type, lenders and jurisdiction type.")]
        public const string CheckListDuplicateEntry = "CheckListDuplicateEntry";

        [Message("Term Frequency must be monthly.")]
        public const string MonthlyTermRequired = "MonthlyTermRequired";

        [Message("Duplicate menu column entry found based on name, deal type and roles.")]
        public static readonly string MenuColumnDuplicateEntry = "MenuColumnDuplicateEntry";

        public static string? GetErrorMessage(string validationCode)
        {
            var field = typeof(ValidationCodes).GetField(validationCode);
            return field?.GetCustomAttribute<MessageAttribute>()?.Message;
        }

        public static ValidationException ValidationException(string propertyName, string validationCode, string? errorMessage = null)
        {
            errorMessage ??= GetErrorMessage(validationCode);
            return new ValidationException(new List<ValidationFailure>
            {
                new (propertyName, errorMessage)
                {
                    ErrorCode = validationCode
                }
            });
        }
    }

    public class ValidationLanguageManager : FluentValidation.Resources.LanguageManager
    {
        public ValidationLanguageManager()
        {
            foreach (var language in new string[] { "en", "en-US" })
            {
                foreach (var field in typeof(ValidationCodes).GetFields())
                {
                    var message = field.GetCustomAttribute<MessageAttribute>()?.Message;
                    if (message is not null)
                    {
                        AddTranslation(language, field.Name, message);
                    }
                }
            }
        }
    }

    [AttributeUsage(AttributeTargets.Field, AllowMultiple = false)]
    public sealed class MessageAttribute : Attribute
    {
        public string Message { get; }

        public MessageAttribute(string message)
        {
            Message = message;
        }
    }
}

/*************** Default Error codes and message ******************************
"EmailValidator" => "'{PropertyName}' is not a valid email address."
"GreaterThanOrEqualValidator" => "'{PropertyName}' must be greater than or equal to '{ComparisonValue}'."
"GreaterThanValidator" => "'{PropertyName}' must be greater than '{ComparisonValue}'."
"LengthValidator" => "'{PropertyName}' must be between {MinLength} and {MaxLength} characters. You entered {TotalLength} characters."
"MinimumLengthValidator" => "The length of '{PropertyName}' must be at least {MinLength} characters. You entered {TotalLength} characters."
"MaximumLengthValidator" => "The length of '{PropertyName}' must be {MaxLength} characters or fewer. You entered {TotalLength} characters."
"LessThanOrEqualValidator" => "'{PropertyName}' must be less than or equal to '{ComparisonValue}'."
"LessThanValidator" => "'{PropertyName}' must be less than '{ComparisonValue}'."
"NotEmptyValidator" => "'{PropertyName}' must not be empty."
"NotEqualValidator" => "'{PropertyName}' must not be equal to '{ComparisonValue}'."
"NotNullValidator" => "'{PropertyName}' must not be empty."
"PredicateValidator" => "The specified condition was not met for '{PropertyName}'."
"AsyncPredicateValidator" => "The specified condition was not met for '{PropertyName}'."
"RegularExpressionValidator" => "'{PropertyName}' is not in the correct format."
"EqualValidator" => "'{PropertyName}' must be equal to '{ComparisonValue}'."
"ExactLengthValidator" => "'{PropertyName}' must be {MaxLength} characters in length. You entered {TotalLength} characters."
"InclusiveBetweenValidator" => "'{PropertyName}' must be between {From} and {To}. You entered {PropertyValue}."
"ExclusiveBetweenValidator" => "'{PropertyName}' must be between {From} and {To} (exclusive). You entered {PropertyValue}."
"CreditCardValidator" => "'{PropertyName}' is not a valid credit card number."
"ScalePrecisionValidator" => "'{PropertyName}' must not be more than {ExpectedPrecision} digits in total, with allowance for {ExpectedScale} decimals. {Digits} digits and {ActualScale} decimals were found."
"EmptyValidator" => "'{PropertyName}' must be empty."
"NullValidator" => "'{PropertyName}' must be empty."
"EnumValidator" => "'{PropertyName}' has a range of values which does not include '{PropertyValue}'."
********************************************************************************/
