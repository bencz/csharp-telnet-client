using System;
using System.Web.UI;
using System.Web.UI.WebControls;
using System.Web.UI.HtmlControls;
using System.Web.UI.Design;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Data;

namespace AutoCoder.DemoControls
{
	namespace WccCoolButton
	{

		public enum CoolButtonMode
		{
			Text = 0,
			Button = 1,
			Image = 2
		}

		[DefaultProperty("Text"), 
		ToolboxData("<{0}:WccCoolButton runat=server></{0}:WccCoolButton>"),
		Designer(typeof( WccCoolButtonDesigner))]
		public class WccCoolButton : System.Web.UI.WebControls.WebControl, INamingContainer
		{
			// The sole event hook that the button exposes.
			public event EventHandler Click;
 
			private LinkButton btnLink;
			private ImageButton btnImage;
			private Button btnButton;

			private Label lblLeftDecoration;
			private Label lblRightDecoration;

			#region DesignProperties
			[Bindable(false), Category("Appearance"), DefaultValue(null), 
			Description("The text that is displayed inside the button.")] 
			public string Text 
			{
				get 
				{
					this.EnsureChildControls();

					string retVal = "";

					switch(CoolButtonMode)
					{
						case CoolButtonMode.Text:
							retVal = btnLink.Text;
							break;

						case CoolButtonMode.Button:
							retVal = btnButton.Text;
							break;

						case CoolButtonMode.Image:
							retVal = btnImage.AlternateText;
							break;
					}
	
					return(retVal);
				}

				set 
				{
					this.EnsureChildControls();

					btnLink.Text = value;
					btnButton.Text = value;
					btnImage.AlternateText = value;
				}
			}
		

			[Bindable(true), Category("Appearance"), DefaultValue("["),
			Description("The text decoration that is displayed on the left side of the button when the CoolButtonMode is set to 'Text'.")] 
			public string LeftDecoration 
			{
				get 
				{
					this.EnsureChildControls();	
					return lblLeftDecoration.Text;
				}

				set 
				{
					this.EnsureChildControls();
					lblLeftDecoration.Text = value;
				}
			}
		

			[Bindable(true), Category("Appearance"), DefaultValue("]"),
			Description("The text decoration that is displayed on the right side of the button when the CoolButtonMode is set to 'Text'.")] 
			public string RightDecoration 
			{
				get 
				{
					this.EnsureChildControls();
					return lblRightDecoration.Text;
				}

				set 
				{
					this.EnsureChildControls();
					lblRightDecoration.Text = value;
				}
			}

		
			[Bindable(false), Category("Appearance"), DefaultValue(null),
			Description("The width of the button text in between the decorations for Text mode buttons. The full width of the button for Button mode buttons. Not used for Image mode buttons.")] 
			public override Unit Width 
			{
				get 
				{
					this.EnsureChildControls();
					return btnLink.Width;
				}

				set 
				{
					this.EnsureChildControls();
					btnLink.Width = value;
					btnButton.Width = value;
				}
			}
		

			[Bindable(false), Category("Appearance"),
			Description("The mode of the button, use Text for a decorated text button, Button for a normal submit button style, or Image for an Image button style.")] 
			public CoolButtonMode CoolButtonMode 
			{
				get 
				{
					CoolButtonMode retVal;

					if(ViewState["eCoolButtonMode"] == null)
						retVal = CoolButtonMode.Text;
					else
						retVal = (CoolButtonMode) ViewState["eCoolButtonMode"];

					return(retVal);
				
				}

				set 
				{
					ViewState["eCoolButtonMode"] = value;

					this.EnsureChildControls();

					HtmlTable t = (HtmlTable) Controls[0];
				
					switch(CoolButtonMode)
					{
						case CoolButtonMode.Button:
							this.btnLink.Visible = false;
							this.btnButton.Visible = true;
							this.btnImage.Visible = false;

							t.Rows[0].Cells[0].Visible = false;
							t.Rows[0].Cells[2].Visible = false;
							break;

						case CoolButtonMode.Image:
							this.btnLink.Visible = false;
							this.btnButton.Visible = false;
							this.btnImage.Visible = true;

							t.Rows[0].Cells[0].Visible = false;
							t.Rows[0].Cells[2].Visible = false;
							break;

						default: // The control is in CoolButtonMode.Text mode.
							this.btnLink.Visible = true;
							this.btnButton.Visible = false;
							this.btnImage.Visible = false;

							t.Rows[0].Cells[0].Visible = true;
							t.Rows[0].Cells[2].Visible = true;
							break;
					}
				}
			}


			[Bindable(false), Category("Appearance"), DefaultValue(""),
			Description("The URL of the image to be displayed for the button. The Text property will become the alt text tag for the image button.")] 
			public string ImageUrl 
			{
				get 
				{
					this.EnsureChildControls();
					return btnImage.ImageUrl;
				}

				set 
				{
					this.EnsureChildControls();
					btnImage.ImageUrl = value;
				}
			}
			#endregion

			/// <summary>
			/// Add the child controls to the container, sizing it to the User's specifications.
			/// </summary>
			protected override void CreateChildControls() 
			{
				// Setup the controls on the page.
				btnLink = new LinkButton();
				btnImage = new ImageButton();
				btnButton = new Button();
				lblLeftDecoration = new Label();
				lblRightDecoration = new Label();

				// Setup the events on the page.
				btnLink.Click += new EventHandler(this.OnBtnLink_Click);
				btnButton.Click += new EventHandler(this.OnBtnButton_Click);
				btnImage.Click += new ImageClickEventHandler(this.OnBtnImage_Click);

				HtmlTable table = new HtmlTable();
				HtmlTableRow newRow;
				HtmlTableCell newCell;

				// Make sure that the composite control flows with the surrounding text properly.
				table.Border = 0;
				table.Style.Add("DISPLAY", "inline");
				table.Style.Add("VERTICAL-ALIGN", "middle");

				newRow = new HtmlTableRow();
		
				newCell = new HtmlTableCell();
				newCell.Controls.Add(lblLeftDecoration);				
				newRow.Cells.Add(newCell);

				newCell = new HtmlTableCell();
				newCell.Align = "center";
			
				// Add all the buttons to the control, so that if they are switched 
				// programatically, the event handlers will stay linked. If the controls
				// are not included in the Controls collection, then the event handling
				// doesn't persist. We will use the visibility to determine which one is 
				// actually rendered for the user to see.
				newCell.Controls.Add(btnLink);
				newCell.Controls.Add(btnButton);
				newCell.Controls.Add(btnImage);

				newRow.Cells.Add(newCell);

				newCell = new HtmlTableCell();
				newCell.Controls.Add(lblRightDecoration);
				newRow.Cells.Add(newCell);

				if(newRow.Cells.Count > 0)
					table.Rows.Add(newRow);
			
				Controls.Add(table);

				// Setup the defaults for the controls.
				this.LeftDecoration = "[";
				this.RightDecoration = "]";
				this.CoolButtonMode = CoolButtonMode.Text;
			}
		

			/// <summary>
			/// This delegate is called when the CoolButtonMode is set to Text.
			/// It's only job is to forward the event to any registered handelers that
			/// are encapsulating this control, including parent composite controls, or 
			/// the page itself.
			/// </summary>
			/// <param name="sender">The sender of the event</param>
			/// <param name="e">An EventArgs object.</param>
			protected virtual void OnBtnLink_Click(object sender, EventArgs e)
			{
				if (Click != null) 
				{
					Click(this, e);
				}  
			}


			/// <summary>
			/// This delegate is called when the CoolButtonMode is set to Button.
			/// It's only job is to forward the event to any registered handelers that
			/// are encapsulating this control, including parent composite controls, or 
			/// the page itself.
			/// </summary>
			/// <param name="sender">The sender of the event</param>
			/// <param name="e">An EventArgs object.</param>
			protected virtual void OnBtnButton_Click(object sender, EventArgs e)
			{
				if (Click != null) 
				{
					Click(this, e);
				}  
			}


			/// <summary>
			/// This delegate is called when the CoolButtonMode is set to Image.
			/// It's only job is to forward the event to any registered handelers that
			/// are encapsulating this control, including parent composite controls, or 
			/// the page itself.
			/// </summary>
			/// <param name="sender">The sender of the event</param>
			/// <param name="e">An EventArgs object.</param>
			protected virtual void OnBtnImage_Click(object sender, ImageClickEventArgs e)
			{
				if (Click != null) 
				{
					Click(this, e);
				}  
			}
		}
	}


	/*********************************************************************
	 * 
	 * The control designer.
	 * 
	 *********************************************************************/
		public class WccCoolButtonDesigner : ControlDesigner
		{
			/// <summary>
			/// Returns a design view of the control as rendered by the control itself.
			/// </summary>
			/// <returns>The HTML of the design time control.</returns>
			public override string GetDesignTimeHtml()
			{
				WccCoolButton.WccCoolButton cb = (WccCoolButton.WccCoolButton) Component;

				// If there are no controls, then it's the first time through the 
				// designer, so set the text to the unique id. This will also 
				// cause EnsureChildControls() to be called in Text(), which will
				// build out the rest of the control.
				if(cb.Controls.Count == 0)
					cb.Text = cb.UniqueID;
			
				StringWriter sw = new StringWriter();
				HtmlTextWriter tw = new HtmlTextWriter(sw);

				cb.RenderBeginTag(tw);
				cb.RenderControl(tw);
				cb.RenderEndTag(tw);
				
				return(sw.ToString());	
			}
		}

	}
