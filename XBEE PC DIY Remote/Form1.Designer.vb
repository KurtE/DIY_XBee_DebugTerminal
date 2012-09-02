<Global.Microsoft.VisualBasic.CompilerServices.DesignerGenerated()> _
Partial Class Form1
    Inherits System.Windows.Forms.Form

    'Form overrides dispose to clean up the component list.
    <System.Diagnostics.DebuggerNonUserCode()> _
    Protected Overrides Sub Dispose(ByVal disposing As Boolean)
        Try
            If disposing AndAlso components IsNot Nothing Then
                components.Dispose()
            End If
        Finally
            MyBase.Dispose(disposing)
        End Try
    End Sub

    'Required by the Windows Form Designer
    Private components As System.ComponentModel.IContainer

    'NOTE: The following procedure is required by the Windows Form Designer
    'It can be modified using the Windows Form Designer.  
    'Do not modify it using the code editor.
    <System.Diagnostics.DebuggerStepThrough()> _
    Private Sub InitializeComponent()
        Me.components = New System.ComponentModel.Container()
        Me.ComGB = New System.Windows.Forms.GroupBox()
        Me.ComLB = New System.Windows.Forms.ComboBox()
        Me.GroupBox1 = New System.Windows.Forms.GroupBox()
        Me.XBeeDL = New System.Windows.Forms.ComboBox()
        Me.Connect = New System.Windows.Forms.Button()
        Me.GroupBox2 = New System.Windows.Forms.GroupBox()
        Me.XBeeMy = New System.Windows.Forms.TextBox()
        Me.CommThread = New System.ComponentModel.BackgroundWorker()
        Me.Discover = New System.Windows.Forms.Button()
        Me.LCDLB = New System.Windows.Forms.ListBox()
        Me.LCDGroup = New System.Windows.Forms.GroupBox()
        Me.PMTerminal = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.LBTerminalSelectAll = New System.Windows.Forms.ToolStripMenuItem()
        Me.LBTerminalCopytoClipBoard = New System.Windows.Forms.ToolStripMenuItem()
        Me.LBTerminalClear = New System.Windows.Forms.ToolStripMenuItem()
        Me.LCDLBClear = New System.Windows.Forms.ToolStripMenuItem()
        Me.XBeeDLContextMenu = New System.Windows.Forms.ContextMenuStrip(Me.components)
        Me.XBDLCM_DeleteItem = New System.Windows.Forms.ToolStripMenuItem()
        Me.XBDLCM_Clear = New System.Windows.Forms.ToolStripMenuItem()
        Me.TBTerminal = New System.Windows.Forms.TextBox()
        Me.Timer1 = New System.Windows.Forms.Timer(Me.components)
        Me.APIMode = New System.Windows.Forms.CheckBox()
        Me.ComGB.SuspendLayout()
        Me.GroupBox1.SuspendLayout()
        Me.GroupBox2.SuspendLayout()
        Me.LCDGroup.SuspendLayout()
        Me.PMTerminal.SuspendLayout()
        Me.XBeeDLContextMenu.SuspendLayout()
        Me.SuspendLayout()
        '
        'ComGB
        '
        Me.ComGB.Controls.Add(Me.ComLB)
        Me.ComGB.Location = New System.Drawing.Point(445, 12)
        Me.ComGB.Name = "ComGB"
        Me.ComGB.Size = New System.Drawing.Size(122, 47)
        Me.ComGB.TabIndex = 12
        Me.ComGB.TabStop = False
        Me.ComGB.Text = "Comm Port"
        '
        'ComLB
        '
        Me.ComLB.FormattingEnabled = True
        Me.ComLB.Location = New System.Drawing.Point(1, 19)
        Me.ComLB.Name = "ComLB"
        Me.ComLB.Size = New System.Drawing.Size(122, 21)
        Me.ComLB.TabIndex = 1
        '
        'GroupBox1
        '
        Me.GroupBox1.Controls.Add(Me.XBeeDL)
        Me.GroupBox1.Location = New System.Drawing.Point(15, 9)
        Me.GroupBox1.Name = "GroupBox1"
        Me.GroupBox1.Size = New System.Drawing.Size(272, 47)
        Me.GroupBox1.TabIndex = 14
        Me.GroupBox1.TabStop = False
        Me.GroupBox1.Text = "XBEE Destination Low"
        '
        'XBeeDL
        '
        Me.XBeeDL.FormattingEnabled = True
        Me.XBeeDL.Location = New System.Drawing.Point(1, 19)
        Me.XBeeDL.Name = "XBeeDL"
        Me.XBeeDL.Size = New System.Drawing.Size(265, 21)
        Me.XBeeDL.TabIndex = 2
        '
        'Connect
        '
        Me.Connect.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Connect.Location = New System.Drawing.Point(615, 12)
        Me.Connect.Name = "Connect"
        Me.Connect.Size = New System.Drawing.Size(97, 23)
        Me.Connect.TabIndex = 15
        Me.Connect.Text = "Connect"
        Me.Connect.UseVisualStyleBackColor = True
        '
        'GroupBox2
        '
        Me.GroupBox2.Controls.Add(Me.XBeeMy)
        Me.GroupBox2.Location = New System.Drawing.Point(293, 12)
        Me.GroupBox2.Name = "GroupBox2"
        Me.GroupBox2.Size = New System.Drawing.Size(134, 47)
        Me.GroupBox2.TabIndex = 16
        Me.GroupBox2.TabStop = False
        Me.GroupBox2.Text = "XBEE My"
        '
        'XBeeMy
        '
        Me.XBeeMy.Location = New System.Drawing.Point(0, 19)
        Me.XBeeMy.Name = "XBeeMy"
        Me.XBeeMy.Size = New System.Drawing.Size(122, 20)
        Me.XBeeMy.TabIndex = 15
        '
        'CommThread
        '
        Me.CommThread.WorkerReportsProgress = True
        Me.CommThread.WorkerSupportsCancellation = True
        '
        'Discover
        '
        Me.Discover.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.Discover.Location = New System.Drawing.Point(616, 41)
        Me.Discover.Name = "Discover"
        Me.Discover.Size = New System.Drawing.Size(96, 23)
        Me.Discover.TabIndex = 48
        Me.Discover.Text = "Discover DLs"
        Me.Discover.UseVisualStyleBackColor = True
        '
        'LCDLB
        '
        Me.LCDLB.Dock = System.Windows.Forms.DockStyle.Fill
        Me.LCDLB.FormattingEnabled = True
        Me.LCDLB.Location = New System.Drawing.Point(3, 16)
        Me.LCDLB.Name = "LCDLB"
        Me.LCDLB.SelectionMode = System.Windows.Forms.SelectionMode.MultiSimple
        Me.LCDLB.Size = New System.Drawing.Size(715, 219)
        Me.LCDLB.TabIndex = 0
        '
        'LCDGroup
        '
        Me.LCDGroup.Anchor = CType((((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Bottom) _
            Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.LCDGroup.Controls.Add(Me.LCDLB)
        Me.LCDGroup.Location = New System.Drawing.Point(15, 97)
        Me.LCDGroup.Name = "LCDGroup"
        Me.LCDGroup.Size = New System.Drawing.Size(721, 238)
        Me.LCDGroup.TabIndex = 43
        Me.LCDGroup.TabStop = False
        Me.LCDGroup.Text = "LCD"
        '
        'PMTerminal
        '
        Me.PMTerminal.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.LBTerminalSelectAll, Me.LBTerminalCopytoClipBoard, Me.LBTerminalClear})
        Me.PMTerminal.Name = "PMTerminal"
        Me.PMTerminal.Size = New System.Drawing.Size(123, 70)
        '
        'LBTerminalSelectAll
        '
        Me.LBTerminalSelectAll.Name = "LBTerminalSelectAll"
        Me.LBTerminalSelectAll.Size = New System.Drawing.Size(152, 22)
        Me.LBTerminalSelectAll.Text = "Select All"
        '
        'LBTerminalCopytoClipBoard
        '
        Me.LBTerminalCopytoClipBoard.Name = "LBTerminalCopytoClipBoard"
        Me.LBTerminalCopytoClipBoard.Size = New System.Drawing.Size(152, 22)
        Me.LBTerminalCopytoClipBoard.Text = "Copy"
        '
        'LBTerminalClear
        '
        Me.LBTerminalClear.Name = "LBTerminalClear"
        Me.LBTerminalClear.Size = New System.Drawing.Size(152, 22)
        Me.LBTerminalClear.Text = "Clear"
        '
        'LCDLBClear
        '
        Me.LCDLBClear.Name = "LCDLBClear"
        Me.LCDLBClear.Size = New System.Drawing.Size(152, 22)
        Me.LCDLBClear.Text = "Clear"
        '
        'XBeeDLContextMenu
        '
        Me.XBeeDLContextMenu.Items.AddRange(New System.Windows.Forms.ToolStripItem() {Me.XBDLCM_DeleteItem, Me.XBDLCM_Clear})
        Me.XBeeDLContextMenu.Name = "PMTerminal"
        Me.XBeeDLContextMenu.Size = New System.Drawing.Size(135, 48)
        '
        'XBDLCM_DeleteItem
        '
        Me.XBDLCM_DeleteItem.Name = "XBDLCM_DeleteItem"
        Me.XBDLCM_DeleteItem.Size = New System.Drawing.Size(134, 22)
        Me.XBDLCM_DeleteItem.Text = "Delete Item"
        '
        'XBDLCM_Clear
        '
        Me.XBDLCM_Clear.Name = "XBDLCM_Clear"
        Me.XBDLCM_Clear.Size = New System.Drawing.Size(134, 22)
        Me.XBDLCM_Clear.Text = "Clear List"
        '
        'TBTerminal
        '
        Me.TBTerminal.AcceptsReturn = True
        Me.TBTerminal.Anchor = CType(((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Left) _
            Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.TBTerminal.Location = New System.Drawing.Point(16, 71)
        Me.TBTerminal.Name = "TBTerminal"
        Me.TBTerminal.Size = New System.Drawing.Size(531, 20)
        Me.TBTerminal.TabIndex = 49
        '
        'Timer1
        '
        Me.Timer1.Interval = 250
        '
        'APIMode
        '
        Me.APIMode.Anchor = CType((System.Windows.Forms.AnchorStyles.Top Or System.Windows.Forms.AnchorStyles.Right), System.Windows.Forms.AnchorStyles)
        Me.APIMode.AutoSize = True
        Me.APIMode.Location = New System.Drawing.Point(616, 74)
        Me.APIMode.Name = "APIMode"
        Me.APIMode.Size = New System.Drawing.Size(73, 17)
        Me.APIMode.TabIndex = 1
        Me.APIMode.Text = "API Mode"
        Me.APIMode.UseVisualStyleBackColor = True
        '
        'Form1
        '
        Me.AutoScaleDimensions = New System.Drawing.SizeF(6.0!, 13.0!)
        Me.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font
        Me.ClientSize = New System.Drawing.Size(744, 342)
        Me.Controls.Add(Me.APIMode)
        Me.Controls.Add(Me.TBTerminal)
        Me.Controls.Add(Me.GroupBox1)
        Me.Controls.Add(Me.GroupBox2)
        Me.Controls.Add(Me.Discover)
        Me.Controls.Add(Me.Connect)
        Me.Controls.Add(Me.ComGB)
        Me.Controls.Add(Me.LCDGroup)
        Me.MinimumSize = New System.Drawing.Size(760, 380)
        Me.Name = "Form1"
        Me.Text = "XBEE DIY Debug Terminal"
        Me.ComGB.ResumeLayout(False)
        Me.GroupBox1.ResumeLayout(False)
        Me.GroupBox2.ResumeLayout(False)
        Me.GroupBox2.PerformLayout()
        Me.LCDGroup.ResumeLayout(False)
        Me.PMTerminal.ResumeLayout(False)
        Me.XBeeDLContextMenu.ResumeLayout(False)
        Me.ResumeLayout(False)
        Me.PerformLayout()

    End Sub
    Friend WithEvents ComGB As System.Windows.Forms.GroupBox
    Friend WithEvents ComLB As System.Windows.Forms.ComboBox
    Friend WithEvents GroupBox1 As System.Windows.Forms.GroupBox
    Friend WithEvents Connect As System.Windows.Forms.Button
    Friend WithEvents GroupBox2 As System.Windows.Forms.GroupBox
    Friend WithEvents XBeeMy As System.Windows.Forms.TextBox
    Friend WithEvents CommThread As System.ComponentModel.BackgroundWorker
    Friend WithEvents Discover As System.Windows.Forms.Button
    Friend WithEvents XBeeDL As System.Windows.Forms.ComboBox
    Friend WithEvents LCDLB As System.Windows.Forms.ListBox
    Friend WithEvents LCDGroup As System.Windows.Forms.GroupBox
    Friend WithEvents PMTerminal As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents LCDLBClear As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LBTerminalSelectAll As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LBTerminalCopytoClipBoard As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents LBTerminalClear As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents XBeeDLContextMenu As System.Windows.Forms.ContextMenuStrip
    Friend WithEvents XBDLCM_DeleteItem As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents XBDLCM_Clear As System.Windows.Forms.ToolStripMenuItem
    Friend WithEvents TBTerminal As System.Windows.Forms.TextBox
    Friend WithEvents Timer1 As System.Windows.Forms.Timer
    Friend WithEvents APIMode As System.Windows.Forms.CheckBox

End Class
