using System;
using System.Web;
using System.ComponentModel;
using System.Web.UI;
using System.Web.UI.WebControls;
using FeedbackFormWebApp.Models;
using FeedbackFormWebApp.Services;
using FeedbackFormWebApp.Utils;


namespace FeedbackFormWebApp.Controls
{
    public class FeedbackEventArgs : EventArgs
    {
        public string Name { get; set; }
        public string Email { get; set; }
        public string Category { get; set; }
        public string Message { get; set; }
        public DateTime SubmittedAt { get; set; }
    }

    [DefaultProperty("HeaderText")]
    [ToolboxData("<{0}:FeedbackFormControl runat=server></{0}:FeedbackFormControl>")]
    public class FeedbackFormControl : CompositeControl, INamingContainer
    {
        // Templates
        [TemplateContainer(typeof(FeedbackFormControl))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate HeaderTemplate { get; set; }

        [TemplateContainer(typeof(FeedbackFormControl))]
        [PersistenceMode(PersistenceMode.InnerProperty)]
        public ITemplate FooterTemplate { get; set; }

        // Appearance / Behavior Properties
        [Category("Appearance")]
        [DefaultValue(true)]
        public bool ShowHeader
        {
            get => (bool?)(ViewState["ShowHeader"]) ?? true;
            set => ViewState["ShowHeader"] = value;
        }

        [Category("Appearance")]
        [DefaultValue("Feedback")]
        public string HeaderText
        {
            get => (string)(ViewState["HeaderText"] ?? "Feedback");
            set => ViewState["HeaderText"] = value;
        }

        [Category("Behavior")]
        [DefaultValue(true)]
        public bool EnableEmailValidation
        {
            get => (bool?)(ViewState["EnableEmailValidation"]) ?? true;
            set => ViewState["EnableEmailValidation"] = value;
        }

        [Category("Behavior")]
        [DefaultValue(false)]
        public bool EnableAjax
        {
            get => (bool?)(ViewState["EnableAjax"]) ?? false;
            set => ViewState["EnableAjax"] = value;
        }

        // Legacy properties (kept for backward compatibility but not used)
        [Browsable(false)]
        public bool SaveToFile { get; set; } = false;

        [Browsable(false)]
        public bool SaveToMemory { get; set; } = false;

        [Browsable(false)]
        public string FilePath { get; set; } = "~/App_Data/feedback.txt";

        // Child Controls
        private Panel _outerWrapper;
        private ValidationSummary _validationSummary;
        private TextBox _txtName, _txtEmail, _txtMessage, _txtCaptcha;
        private DropDownList _ddlCategory;
        private Button _btnSubmit;
        private CustomValidator _cvCaptcha;
        private Literal _litResult;

        // custom event the control will raise on successful submission
        [Category("Action")]
        public event EventHandler<FeedbackEventArgs> FeedbackSubmitted;

        // CAPTCHA value stored in ViewState
        private string CaptchaValue
        {
            get => (string)ViewState["CaptchaValue"];
            set => ViewState["CaptchaValue"] = value;
        }

        protected override void CreateChildControls()   //control's entire visual tree in code
        {
            Controls.Clear();
            Control container = this;
            UpdatePanel updatePanel = null;

            // Setup AJAX UpdatePanel if enabled
            if (EnableAjax)
            {
                var scriptManager = ScriptManager.GetCurrent(Page);
                if (scriptManager == null && Page?.Form != null)
                {
                    var autoScriptManager = new ScriptManager { ID = "sm_Auto" };
                    Page.Form.Controls.AddAt(0, autoScriptManager);
                }

                updatePanel = new UpdatePanel
                {
                    ID = "upFeedback",
                    UpdateMode = UpdatePanelUpdateMode.Conditional
                };
                this.Controls.Add(updatePanel);
                container = updatePanel.ContentTemplateContainer;
            }

            // Main wrapper
            _outerWrapper = new Panel { CssClass = "ff-wrapper" };
            container.Controls.Add(_outerWrapper);

            // Header section
            CreateHeader();

            // Validation summary
            CreateValidationSummary();

            // Form fields
            CreateFormFields();

            // Footer section
            CreateFooter();

            // Submit button and result
            CreateActions();
        }

        private void CreateHeader()
        {
            if (HeaderTemplate != null)
            {
                var headerContainer = new Panel { CssClass = "ff-header" };
                HeaderTemplate.InstantiateIn(headerContainer);
                _outerWrapper.Controls.Add(headerContainer);
            }
            else if (ShowHeader)
            {
                var headerLiteral = new Literal
                {
                    Text = $"<div class='ff-header'>{System.Net.WebUtility.HtmlEncode(HeaderText)}</div>"
                };
                _outerWrapper.Controls.Add(headerLiteral);
            }
        }

        private void CreateValidationSummary()
        {
            _validationSummary = new ValidationSummary
            {
                ID = "valSummary",
                CssClass = "validation-summary",
                DisplayMode = ValidationSummaryDisplayMode.BulletList,
                ShowSummary = true,
                HeaderText = "Please correct the following errors:"
            };
            _outerWrapper.Controls.Add(_validationSummary);
        }

        private void CreateFormFields()
        {
            // Name field
            _outerWrapper.Controls.Add(CreateInputRow("Name", out _txtName, true));

            // Email field with validation
            _outerWrapper.Controls.Add(CreateInputRow("Email", out _txtEmail, true));
            if (EnableEmailValidation)
            {
                var emailValidator = new RegularExpressionValidator
                {
                    ID = "revEmail",
                    ControlToValidate = _txtEmail.ID,
                    ErrorMessage = "Please enter a valid email address.",
                    ValidationExpression = @"^[^@\s]+@[^@\s]+\.[^@\s]+$",
                    CssClass = "ff-error",
                    Display = ValidatorDisplay.Dynamic
                };
                ((Control)_txtEmail.Parent).Controls.Add(emailValidator);
            }

            // Category dropdown
            CreateCategoryField();

            // Message field
            _outerWrapper.Controls.Add(CreateInputRow("Message", out _txtMessage, true, TextBoxMode.MultiLine, 5));

            // CAPTCHA field
            CreateCaptchaField();
        }

        private Control CreateInputRow(string labelText, out TextBox textBox, bool required,
            TextBoxMode mode = TextBoxMode.SingleLine, int rows = 1)
        {
            var row = new Panel { CssClass = "ff-row" };

            // Label
            var labelLiteral = new Literal
            {
                Text = $"<div class='ff-label'>{System.Net.WebUtility.HtmlEncode(labelText)}</div>"
            };
            row.Controls.Add(labelLiteral);

            // Input container
            var inputContainer = new Panel { CssClass = "ff-input" };

            // TextBox
            textBox = new TextBox
            {
                ID = "txt" + labelText.Replace(" ", ""),
                TextMode = mode
            };

            if (mode == TextBoxMode.MultiLine && rows > 1)
                textBox.Rows = rows;

            inputContainer.Controls.Add(textBox);

            // Required field validator
            if (required)
            {
                var requiredValidator = new RequiredFieldValidator
                {
                    ID = "rfv" + labelText.Replace(" ", ""),
                    ControlToValidate = textBox.ID,
                    ErrorMessage = $"{labelText} is required.",
                    CssClass = "ff-error",
                    Display = ValidatorDisplay.Dynamic
                };
                inputContainer.Controls.Add(requiredValidator);
            }

            row.Controls.Add(inputContainer);
            return row;
        }

        private void CreateCategoryField()
        {
            var categoryRow = new Panel { CssClass = "ff-row" };

            // Label
            categoryRow.Controls.Add(new Literal
            {
                Text = "<div class='ff-label'>Category</div>"
            });

            // Input container
            var inputContainer = new Panel { CssClass = "ff-input" };

            // Dropdown
            _ddlCategory = new DropDownList { ID = "ddlCategory" };
            _ddlCategory.Items.Add(new ListItem("-- Select Category --", ""));
            _ddlCategory.Items.Add(new ListItem("Suggestion", "Suggestion"));
            _ddlCategory.Items.Add(new ListItem("Bug Report", "Bug"));
            _ddlCategory.Items.Add(new ListItem("Complaint", "Complaint"));
            _ddlCategory.Items.Add(new ListItem("Other", "Other"));

            inputContainer.Controls.Add(_ddlCategory);

            // Required validator
            var categoryValidator = new RequiredFieldValidator
            {
                ID = "rfvCategory",
                ControlToValidate = _ddlCategory.ID,
                InitialValue = "",
                ErrorMessage = "Please choose a category.",
                CssClass = "ff-error",
                Display = ValidatorDisplay.Dynamic
            };
            inputContainer.Controls.Add(categoryValidator);

            categoryRow.Controls.Add(inputContainer);
            _outerWrapper.Controls.Add(categoryRow);
        }

        private void CreateCaptchaField()
        {
            EnsureCaptcha();

            var captchaRow = new Panel { CssClass = "ff-row" };

            // Label
            captchaRow.Controls.Add(new Literal
            {
                Text = "<div class='ff-label'>CAPTCHA</div>"
            });

            // Input container
            var inputContainer = new Panel { CssClass = "ff-input" };

            // CAPTCHA display
            var captchaDisplay = new Literal
            {
                Text = $"<div style='margin-bottom:8px;font-weight:600;'>Enter the code: <span class='ff-captcha-label'>{System.Net.WebUtility.HtmlEncode(CaptchaValue)}</span></div>"
            };
            inputContainer.Controls.Add(captchaDisplay);

            // CAPTCHA input
            _txtCaptcha = new TextBox { ID = "txtCaptcha", MaxLength = 5 };
            inputContainer.Controls.Add(_txtCaptcha);

            // Required validator
            var captchaRequiredValidator = new RequiredFieldValidator
            {
                ID = "rfvCaptcha",
                ControlToValidate = _txtCaptcha.ID,
                ErrorMessage = "Please enter the CAPTCHA code.",
                CssClass = "ff-error",
                Display = ValidatorDisplay.Dynamic
            };
            inputContainer.Controls.Add(captchaRequiredValidator);

            // Custom validator for CAPTCHA verification
            _cvCaptcha = new CustomValidator
            {
                ID = "cvCaptcha",
                ControlToValidate = _txtCaptcha.ID,
                ErrorMessage = "CAPTCHA code does not match.",
                CssClass = "ff-error",
                Display = ValidatorDisplay.Dynamic
            };
            _cvCaptcha.ServerValidate += CvCaptcha_ServerValidate;
            inputContainer.Controls.Add(_cvCaptcha);

            captchaRow.Controls.Add(inputContainer);
            _outerWrapper.Controls.Add(captchaRow);
        }

        private void CreateFooter()
        {
            if (FooterTemplate != null)
            {
                var footerContainer = new Panel { CssClass = "ff-footer" };
                FooterTemplate.InstantiateIn(footerContainer);
                _outerWrapper.Controls.Add(footerContainer);
            }
        }

        private void CreateActions()
        {
            // Actions container
            var actionsContainer = new Panel { CssClass = "ff-actions" };

            // Submit button
            _btnSubmit = new Button
            {
                ID = "btnSubmit",
                Text = "Submit Feedback",
                CssClass = "ff-button ff-button-primary",
                CausesValidation = true,
                UseSubmitBehavior = false
            };
            _btnSubmit.Click += BtnSubmit_Click;
            actionsContainer.Controls.Add(_btnSubmit);

            _outerWrapper.Controls.Add(actionsContainer);

            // Result display
            _litResult = new Literal { ID = "litResult" };
            _outerWrapper.Controls.Add(_litResult);
        }

        private void EnsureCaptcha()
        {
            if (string.IsNullOrEmpty(CaptchaValue))
            {
                const string chars = "23456789ABCDEFGHJKLMNPQRSTUVWXYZ";
                var random = new Random();
                var code = "";
                for (int i = 0; i < 5; i++)
                    code += chars[random.Next(chars.Length)];
                CaptchaValue = code;
            }
        }

        private void CvCaptcha_ServerValidate(object source, ServerValidateEventArgs args)
        {
            args.IsValid = string.Equals(
                (args.Value ?? "").Trim(),
                (CaptchaValue ?? "").Trim(),
                StringComparison.OrdinalIgnoreCase
            );
        }
        public IFeedbackRepository FeedbackRepository { get; set; } = new FeedbackRepository();

        private void BtnSubmit_Click(object sender, EventArgs e)
        {
            EnsureChildControls();   //Ensures sab controls created
            Page.Validate();    //Triggers all validation

            if (!Page.IsValid)
                return;

            // Additional server-side CAPTCHA validation
            if (_cvCaptcha != null)
            {
                var captchaArgs = new ServerValidateEventArgs(_txtCaptcha.Text, false);
                CvCaptcha_ServerValidate(_cvCaptcha, captchaArgs);
                if (!captchaArgs.IsValid)
                {
                    _cvCaptcha.IsValid = false;
                    return;
                }
            }
            var indiaTimeZone = TimeZoneInfo.FindSystemTimeZoneById("India Standard Time");
            var indiaTime = TimeZoneInfo.ConvertTimeFromUtc(DateTime.UtcNow, indiaTimeZone);
            // Create feedback event args
            var feedbackArgs = new FeedbackEventArgs
            {
                Name = _txtName?.Text?.Trim() ?? string.Empty,
                Email = _txtEmail?.Text?.Trim() ?? string.Empty,
                Category = _ddlCategory?.SelectedValue ?? string.Empty,
                Message = _txtMessage?.Text?.Trim() ?? string.Empty,
                SubmittedAt = indiaTime
            };

            try
            {
                // Save to database using EF Create a Feedback object
                var feedback = new Feedback
                {
                    Name = feedbackArgs.Name,
                    Email = feedbackArgs.Email,
                    Category = feedbackArgs.Category,
                    Message = feedbackArgs.Message,
                    SubmittedAt = feedbackArgs.SubmittedAt
                };

                
                FeedbackRepository.Add(feedback); 


                // Log successful submission
                AppLogger.Info($"Feedback submitted successfully - Email: {feedback.Email}, Category: {feedback.Category}");

                // Raise the event for parent page to handle
                FeedbackSubmitted?.Invoke(this, feedbackArgs); // ? mtlb is null check

                // Show success message
                _litResult.Text = @"
                    <div class='result'>
                        <h3>Thank you!</h3>
                        <p>Your feedback has been recorded successfully. We appreciate your input!</p>
                    </div>";

                // Reset form
                ResetForm();
            }
            catch (Exception ex)
            {
                // Log error
                AppLogger.Error("Error saving feedback to database", ex);

                // Show error message
                _litResult.Text = @"
                    <div class='result' style='background:#fff1f2;border-left:4px solid #dc2626;color:#991b1b;'>
                        <h3>Error</h3>
                        <p>Sorry, there was an error saving your feedback. Please try again later.</p>
                    </div>";
            }
        }

        private void ResetForm()
        {
            // Clear all form fields
            if (_txtName != null) _txtName.Text = string.Empty;
            if (_txtEmail != null) _txtEmail.Text = string.Empty;
            if (_txtMessage != null) _txtMessage.Text = string.Empty;
            if (_txtCaptcha != null) _txtCaptcha.Text = string.Empty;
            if (_ddlCategory != null) _ddlCategory.ClearSelection();

            // Generate new CAPTCHA
            CaptchaValue = null;
            EnsureCaptcha();
        }

        public override void RenderBeginTag(HtmlTextWriter writer)
        {
            // Don't render additional container tags
        }

        public override void RenderEndTag(HtmlTextWriter writer)
        {
            // Don't render additional container tags
        }
    }
}