#region Copyright (c) 2002-2003 Charlie Poole
/************************************************************************************
'
' Copyright (c) 2002-2003 Charlie Poole
'
' Later versions may be available at http://charliepoole.org.
'
' This software is provided 'as-is', without any express or implied warranty. In no 
' event will the author be held liable for any damages arising from the use of this 
' software.
' 
' Permission is granted to anyone to use this software for any purpose, including 
' commercial applications, and to alter it and redistribute it freely, subject to the 
' following restrictions:
'
' 1. The origin of this software must not be misrepresented; you must not claim that
' you wrote the original software. If you use this software in a product, you must
' include the following notice in the product documentation and/or other materials
' provided with the distribution.
'
' Portions Copyright (c) 2002-2003 Charlie Poole
'
' 2. Altered source versions must be plainly marked as such, and must not be 
' misrepresented as being the original software.
'
' 3. This notice may not be removed from or altered in any source distribution.
'
'***********************************************************************************/
#endregion

using System;
using System.Drawing;
using System.Windows.Forms;
using System.Runtime.InteropServices;

namespace CP.Windows.Forms
{
	/// <summary>
	/// Summary description for TipWindow.
	/// </summary>
	public class TipWindow : Form
	{
		/// <summary>
		/// Direction in which to expand
		/// </summary>
		public enum ExpansionStyle
		{
			Horizontal,
			Vertical,
			Both
		}

		#region Instance Variables

		/// <summary>
		/// Text we are displaying
		/// </summary>
		private string tipText;

		/// <summary>
		/// The control for which we are showing expanded text
		/// </summary>
		private Control control;

		/// <summary>
		/// The ListBox for which we are showing expanded text.
		/// Null if the control is not a ListBox
		/// </summary>
		private ListBox listbox;

		/// <summary>
		/// The ListBox item index for which we are showing expanded text.
		/// </summary>
		private int itemIndex = -1;

		/// <summary>
		/// Rectangle representing bounds to overlay. For a listbox, this
		/// is a single item rectangle. For other controls, it is usually
		/// the entire client area.
		/// </summary>
		private Rectangle itemBounds;

		/// <summary>
		/// True if we may overlay control or item
		/// </summary>
		private bool overlay = true;
			
		/// <summary>
		/// Directions we are allowed to expand
		/// </summary>
		private ExpansionStyle expansion = ExpansionStyle.Horizontal;

		/// <summary>
		/// Time before automatically closing
		/// </summary>
		private int autoCloseDelay = 0;

		/// <summary>
		/// Timer used for auto-close
		/// </summary>
		private System.Windows.Forms.Timer autoCloseTimer;

		/// <summary>
		/// Time to wait for after mouse leaves
		/// the window or the label before closing.
		/// </summary>
		private int mouseLeaveDelay = 300;

		/// <summary>
		/// Timer used for mouse leave delay
		/// </summary>
		private System.Windows.Forms.Timer mouseLeaveTimer;

		/// <summary>
		/// Rectangle used to draw border
		/// </summary>
		private Rectangle outlineRect;
			
		/// <summary>
		/// Rectangle used to display text
		/// </summary>
		private Rectangle textRect;

		/// <summary>
		/// Indicates whether any clicks should be passed to the underlying control
		/// </summary>
		private bool wantClicks = false;

		#endregion

		#region Construction and Initialization

		public TipWindow( Control control )
		{
			this.control = control;
			this.Owner = control.FindForm();
			this.itemBounds = control.ClientRectangle;

			this.ControlBox = false;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.BackColor = Color.LightYellow;
			this.FormBorderStyle = FormBorderStyle.None;
			this.StartPosition = FormStartPosition.Manual; 			

			this.tipText = control.Text;
			this.Font = control.Font;
		}

		public TipWindow( ListBox listbox, int index ) : this ( listbox )
		{
			this.listbox = listbox;
			this.itemIndex = index;
			this.itemBounds = listbox.GetItemRectangle( index );
			this.tipText = listbox.Items[ index ].ToString();
		}

		private void InitializeComponent()
		{
			// 
			// TipWindow
			// 
			this.AutoScaleBaseSize = new System.Drawing.Size(6, 15);
			this.ClientSize = new System.Drawing.Size(292, 268);
			this.Name = "TipWindow";
			this.ShowInTaskbar = false;

		}

		protected override void OnLoad(System.EventArgs e)
		{
			// At this point, further changes to the properties
			// of the label will have no effect on the tip.

			Point origin = control.Parent.PointToScreen( control.Location );
			origin.Offset( itemBounds.Left, itemBounds.Top );
			if ( !overlay )	origin.Offset( 0, itemBounds.Height );
			this.Location = origin;

			Graphics g = Graphics.FromHwnd( Handle );

			// This works if expanding in both directons
			Size sizeNeeded;
			if ( expansion == ExpansionStyle.Vertical )
				sizeNeeded = Size.Ceiling( g.MeasureString( tipText, Font, itemBounds.Width ) );
			else
				sizeNeeded = Size.Ceiling( g.MeasureString( tipText, Font ) );
				
			if ( expansion == ExpansionStyle.Horizontal )
				sizeNeeded.Height = itemBounds.Height;

			this.ClientSize = sizeNeeded + new Size( 2, 2 );
			this.outlineRect = new Rectangle( 0, 0, sizeNeeded.Width + 1, sizeNeeded.Height + 1 );
			this.textRect = new Rectangle( 1, 1, sizeNeeded.Width, sizeNeeded.Height );

			// Catch mouse leaving the control
			control.MouseLeave += new EventHandler( control_MouseLeave );

			// Catch the form that holds the control closing
			control.FindForm().Closed += new EventHandler( control_FormClosed );

			// Make sure we'll fit on the screen
			Screen screen = Screen.FromControl( control );
			if ( this.Right > screen.WorkingArea.Right )
				this.Left = screen.WorkingArea.Right - this.Width;

			if ( this.Bottom > screen.WorkingArea.Bottom )
			{
				if ( overlay )
					this.Top = screen.WorkingArea.Bottom - this.Height;
				else if ( control.Top > this.Height )
					this.Top = origin.Y - control.Height - this.Height;
			}

			if ( autoCloseDelay > 0 )
			{
				autoCloseTimer = new System.Windows.Forms.Timer();
				autoCloseTimer.Interval = autoCloseDelay;
				autoCloseTimer.Tick += new EventHandler( OnAutoClose );
				autoCloseTimer.Start();
			}
		}

		#endregion

		#region Properties

		public bool Overlay
		{
			get { return overlay; }
			set { overlay = value; }
		}

		public ExpansionStyle Expansion
		{
			get { return expansion; }
			set { expansion = value; }
		}

		public int AutoCloseDelay
		{
			get { return autoCloseDelay; }
			set { autoCloseDelay = value; }
		}

		public int MouseLeaveDelay
		{
			get { return mouseLeaveDelay; }
			set { mouseLeaveDelay = value; }
		}

		public string TipText
		{
			get { return tipText; }
			set { tipText = value; }
		}

		public Rectangle ItemBounds
		{
			get { return itemBounds; }
			set { itemBounds = value; }
		}

		public bool WantClicks
		{
			get { return wantClicks; }
			set { wantClicks = value; }
		}

		private void CopyToClipboard( object sender, EventArgs e )
		{
			Clipboard.SetDataObject( this.Text );
		}

		#endregion

		#region Event Handlers

		protected override void OnPaint(System.Windows.Forms.PaintEventArgs e)
		{
			base.OnPaint( e );
				
			Graphics g = e.Graphics;
			g.DrawRectangle( Pens.Black, outlineRect );
			g.DrawString( tipText, Font, Brushes.Black, textRect );
		}

		private void OnAutoClose( object sender, System.EventArgs e )
		{
			this.Close();
		}

		protected override void OnMouseEnter(System.EventArgs e)
		{
			if ( mouseLeaveTimer != null )
			{
				mouseLeaveTimer.Stop();
				mouseLeaveTimer.Dispose();
				System.Diagnostics.Debug.WriteLine( "Entered TipWindow - stopped mouseLeaveTimer" );
			}
		}

		protected override void OnMouseLeave(System.EventArgs e)
		{
			if ( mouseLeaveDelay > 0  )
			{
				mouseLeaveTimer = new System.Windows.Forms.Timer();
				mouseLeaveTimer.Interval = mouseLeaveDelay;
				mouseLeaveTimer.Tick += new EventHandler( OnAutoClose );
				mouseLeaveTimer.Start();
				System.Diagnostics.Debug.WriteLine( "Left TipWindow - started mouseLeaveTimer" );
			}
		}

		/// <summary>
		/// The form our label is on closed, so we should. 
		/// </summary>
		private void control_FormClosed( object sender, System.EventArgs e )
		{
			this.Close();
		}

		/// <summary>
		/// The mouse left the label. We ignore if we are
		/// overlaying the label but otherwise start a
		/// delay for closing the window
		/// </summary>
		private void control_MouseLeave( object sender, System.EventArgs e )
		{
			if ( mouseLeaveDelay > 0 && !overlay )
			{
				mouseLeaveTimer = new System.Windows.Forms.Timer();
				mouseLeaveTimer.Interval = mouseLeaveDelay;
				mouseLeaveTimer.Tick += new EventHandler( OnAutoClose );
				mouseLeaveTimer.Start();
				System.Diagnostics.Debug.WriteLine( "Left Control - started mouseLeaveTimer" );
			}
		}

		#endregion
	
		[DllImport("user32.dll")]
		static extern uint SendMessage(
			IntPtr hwnd,
			int msg,
			IntPtr wparam,
			IntPtr lparam
		);
	
		protected override void WndProc(ref Message m)
		{
			uint WM_LBUTTONDOWN = 0x201;
			uint WM_RBUTTONDOWN = 0x204;
			uint WM_MBUTTONDOWN = 0x207;

			if ( m.Msg == WM_LBUTTONDOWN || m.Msg == WM_RBUTTONDOWN || m.Msg == WM_MBUTTONDOWN )
			{
				this.Close();
				SendMessage( control.Handle, m.Msg, m.WParam, m.LParam );
			}
			else
			{
				base.WndProc (ref m);
			}
		}
	}
}
