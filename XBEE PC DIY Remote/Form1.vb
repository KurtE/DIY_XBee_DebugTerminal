Imports System.Text.Encoding 'From VB Pipe
Public Class Form1
    ' This delegate enables asynchronous calls for setting
    ' the text property on a TextBox control.
    Delegate Sub SetTextCallback(ByVal [text] As String)
    Private Shared mut As New Mutex()

    Public SCListBox As New StringCollection        ' use this to communicate between background thread and main thread...

    ' Lets define a bunch of global junk, later should clean up!
    Public fDataHasChanged As Boolean        '
    Public fNewPacketMsgSent As Boolean
    Public fNewPacketMsgMode As Boolean
    Public XBeeCommport As String
    Public XBeeCurDL As UInt32 ' our DL we are working with.
    Public XBeeCurMy As UInt32 ' Our My...
    Public bAPISeqNum As Byte
    Public XBSW As Stopwatch
    Public g_sUserInput As String
    Public g_fSendString As Boolean

    Public PacketHdr(4) As Byte             ' A data packet header

    '    Public Const DATA_PACKET_SIZE = 10  ' Zentas packet size
    Public Const DATA_PACKET_SIZE = 8       ' Kurts packet size
    Public Const XBEE_API_PH_SIZE = 1           ' Packet header size
    Public Const XBEE_ATRESP_DATA_OFFSET = 5    ' Offset to the first data byte in the returned packet.

    Public DataPacket(DATA_PACKET_SIZE) As Byte           'XP 8 -> 10
    Public ButtonsState(2) As Byte              ' we will cache the button state here.
    Public Const PKT_BTNLOW = 0                 ' Low Buttons 0-7
    Public Const PKT_BTNHI = 1                  ' High buttons 8-F
    Public Const PKT_RJOYLR = 2                 ' Right Joystick Up/Down
    Public Const PKT_RJOYUD = 3                 ' Right joystick left/Right
    Public Const PKT_LJOYLR = 4                 ' Left joystick Left/Right
    Public Const PKT_LJOYUD = 5                 ' Left joystick Up/Down
    Public Const PKT_RSLIDER = 6                ' right slider
    Public Const PKT_LSLIDER = 7                ' Left slider
    Public Const PKT_RPOT = 8                   ' right potmeter
    Public Const PKT_LPOT = 9                   ' Left potmeter

    ' Now define our different types of packets
    Public Const XBEE_TRANS_READY = &H1         ' Transmitter is ready for requests*
    Public Const XBEE_TRANS_NOTREADY = &H2      ' Transmitter is exiting transmitting on the sent DL
    Public Const XBEE_TRANS_DATA = &H3          ' Data Packet from Transmitter to Robot*
    Public Const XBEE_TRANS_NEW = &H4           ' New Data Available
    Public Const XBEE_TRANS_SSC_MODE = &H5        ' SSC-MODE
    Public Const XBEE_REQ_SN_NI = &H6           ' Request the serial number and NI string
    Public Const XBEE_TRANS_CHANGED_DATA = &H7    ' We transmite a bit mask with which fields changed plus the bytes that changes

    Public Const XBEE_TRANS_NOTHIN_CHANGED = &H8  ' 
    Public Const XBEE_TRANS_DATA_VERSION = &H9    '  What format of data this transmitter supports. 
    Public Const XBEE_DEBUG_ATTACH = &HA        ' Debug Attach - used to say send debug info to display
    Public Const XBEE_DEBUG_DETACH = &HB        ' End debug output messages...
    Public Const XBEE_DEBUG_STRING = &HC        ' Send a Text string...

    Public Const XBEE_RECV_REQ_DATA = &H80      ' Request Data Packet*
    Public Const XBEE_RECV_REQ_NEW = &H81       ' Request Only New data
    Public Const XBEE_RECV_REQ_NEW_OFF = &H82   ' Request Only New data
    Public Const XBEE_RECV_NEW_THRESH = &H83    ' Set new Data thresholds
    Public Const XBEE_RECV_DISP_VAL = &H84      ' Display a value on line 2
    Public Const XBEE_RECV_DISP_STR = &H85      ' Display a string on line 2
    Public Const XBEE_PLAY_SOUND = &H86  '    Will make sounds on the remote...
    '	<cbExtra> - 2 bytes per sound: Duration <0-255>, Sound: <Freq/25> to make fit in byte...
    Public Const XBEE_SSC_MODE_EXITED = &H87  ' a message sent back to the controller when
    Public Const XBEE_SEND_SN_NI_DATA = &H88  ' Response for REQ_SN_NI - will return

    '========== VB Pipe start =================
    Public Const FILE_ATTRIBUTE_NORMAL = &H80
    Public Const FILE_FLAG_NO_BUFFERING = &H20000000
    Public Const FILE_FLAG_WRITE_THROUGH = &H80000000
    Public Const FILE_FLAG_OVERLAPPED = &H40000000
    Public Const ERROR_OPERATION_ABORTED = 995&
    Public Const ERROR_IO_INCOMPLETE = 996&
    Public Const ERROR_IO_PENDING = 997&

    Public Const PIPE_ACCESS_DUPLEX = &H3
    Public Const PIPE_READMODE_MESSAGE = &H2
    Public Const PIPE_TYPE_MESSAGE = &H4
    Public Const PIPE_WAIT = &H0

    Public Const FILE_BEGIN = 0
    Public Const FILE_CURRENT = 1
    Public Const FILE_END = 2

    Public Const INVALID_HANDLE_VALUE = -1

    Public Const SECURITY_DESCRIPTOR_MIN_LENGTH = (20)
    Public Const SECURITY_DESCRIPTOR_REVISION = (1)

    Public Const INFINITE = -1&

    Private Const QS_KEY = &H1&
    Private Const QS_MOUSEMOVE = &H2&
    Private Const QS_MOUSEBUTTON = &H4&
    Private Const QS_POSTMESSAGE = &H8&
    Private Const QS_TIMER = &H10&
    Private Const QS_PAINT = &H20&
    Private Const QS_SENDMESSAGE = &H40&
    Private Const QS_HOTKEY = &H80&
    Private Const QS_ALLINPUT = (QS_SENDMESSAGE Or QS_PAINT _
             Or QS_TIMER Or QS_POSTMESSAGE Or QS_MOUSEBUTTON _
             Or QS_MOUSEMOVE Or QS_HOTKEY Or QS_KEY)




    Public Structure SECURITY_ATTRIBUTES
        Public nLength As Integer
        Public lpSecurityDescriptor As Integer
        Public bInheritHandle As Integer
    End Structure

    Public Structure OVERLAPPED
        Public Internal As Integer
        Public InternalHigh As Integer
        Public offset As Integer
        Public OffsetHigh As Integer
        Public hEvent As Integer
    End Structure

    Public Const GMEM_FIXED = &H0
    Public Const GMEM_ZEROINIT = &H40
    Public Const GPTR = (GMEM_FIXED Or GMEM_ZEROINIT)

    Declare Function GlobalAlloc Lib "kernel32" ( _
       ByVal wFlags As Integer, ByVal dwBytes As Integer) As Integer
    Declare Function GlobalFree Lib "kernel32" (ByVal hMem As Integer) As Integer
    Declare Function CreateNamedPipe Lib "kernel32" Alias _
       "CreateNamedPipeA" ( _
       ByVal lpName As String, _
       ByVal dwOpenMode As Integer, _
       ByVal dwPipeMode As Integer, _
       ByVal nMaxInstances As Integer, _
       ByVal nOutBufferSize As Integer, _
       ByVal nInBufferSize As Integer, _
       ByVal nDefaultTimeOut As Integer, _
    ByRef lpSecurityAttributes As SECURITY_ATTRIBUTES) As Integer

    Declare Function InitializeSecurityDescriptor Lib "advapi32.dll" ( _
       ByVal pSecurityDescriptor As Integer, _
       ByVal dwRevision As Integer) As Integer

    Declare Function SetSecurityDescriptorDacl Lib "advapi32.dll" ( _
       ByVal pSecurityDescriptor As Integer, _
       ByVal bDaclPresent As Integer, _
       ByVal pDacl As Integer, _
       ByVal bDaclDefaulted As Integer) As Integer

    Declare Function ConnectNamedPipe Lib "kernel32" ( _
        ByVal hNamedPipe As Integer, _
        ByRef lpOverlapped As Overlapped) As Integer

    Declare Function DisconnectNamedPipe Lib "kernel32" ( _
        ByVal hNamedPipe As Integer) As Integer

    Declare Function WriteFile Lib "kernel32" ( _
        ByVal hFile As Integer, _
        ByVal lpBuffer As Byte(), _
        ByVal nNumberOfBytesToWrite As Integer, _
        ByRef lpNumberOfBytesWritten As Integer, _
        ByRef lpOverlapped As Overlapped) As Integer

    Declare Function ReadFile Lib "kernel32" ( _
        ByVal hFile As Integer, _
        ByRef lpBuffer As Byte, _
        ByVal nNumberOfBytesToRead As Integer, _
        ByRef lpNumberOfBytesRead As Integer, _
        ByRef lpOverlapped As Overlapped) As Integer

    Declare Function FlushFileBuffers Lib "kernel32" ( _
        ByVal hFile As Integer) As Integer

    Declare Function CloseHandle Lib "kernel32" ( _
        ByVal hObject As Integer) As Integer

    Declare Function SetFilePointer Lib "kernel32" ( _
        ByVal hFile As Integer, _
        ByVal lDistanceToMove As Integer, _
        ByVal lpDistanceToMoveHigh As Integer, _
        ByVal dwMoveMethod As Integer) As Integer

    Declare Function CreateThread Lib "kernel32" ( _
       ByVal lpSecurityAttributes As Integer, _
       ByVal dwStackSize As Integer, _
       ByVal lpStartAddress As Integer, _
       ByVal lpParameter As Integer, _
       ByVal dwCreationFlags As Integer, _
    ByVal lpThreadId As Integer) As Integer

    Declare Function CreateEvent Lib "kernel32" Alias "CreateEventA" ( _
        ByVal lpEventAttributes As Integer, _
        ByVal bManualReset As Integer, _
        ByVal bInitialState As Integer, _
        ByVal lpName As String) As Integer

    Declare Function WaitForMultipleObjects Lib "kernel32" ( _
        ByVal nCount As Integer, _
        ByVal lpHandles As Integer, _
        ByVal bWaitAll As Integer, _
        ByVal dwMilliseconds As Integer) As Integer

    Declare Function MsgWaitForMultipleObjects Lib "user32" ( _
        ByVal nCount As Integer, _
        ByVal pHandles As Integer, _
        ByVal fWaitAll As Integer, _
        ByVal dwMilliseconds As Integer, _
        ByVal dwWakeMask As Integer) As Integer

    Private pSD As Integer
    Private sa As SECURITY_ATTRIBUTES
    Private hPipe As Integer
    Private Const szPipeName = "\\.\pipe\rrpipe"
    ' size of the pipe buffer used to communicate to/from RR
    Private Const PIPE_BUFFER_SIZE = 4096
    ' buffer size used when reading in variable data
    Private Const DATA_BUFFER = 1024
    ' the maximum length of a variable name to read
    Private Const MAX_VARNAME_SIZE = 64

    ' hold the name used in RR to identify this module
    Private imageName As String
    ' image processed count ... sent back to RR
    Private imageCount As Integer
    ' dimensions of the received image
    Private imageWidth As Integer, imageHeight As Integer
    ' holds the image data
    Private imagePixels() As Byte
    Public RunningWithPipe As Boolean
    Public gbl_hStopEvent As Integer

    Dim variables As New Hashtable

    Private Function intToByte(ByVal i As Integer)

        Dim bArray As Byte() = {i And 255, (i >> 8) And 255, (i >> 16) And 255, (i >> 24) And 255}
        intToByte = bArray

    End Function

    Private Sub Form1_FormClosed(ByVal sender As Object, ByVal e As System.Windows.Forms.FormClosedEventArgs) Handles Me.FormClosed
        My.Settings.MainFormSize = Size

    End Sub
    '========= VB Pipe end==============




    Private Sub Form1_Load(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles MyBase.Load
        ' Show all available COM ports.
        Size = My.Settings.MainFormSize   ' see if we can save and restore the size

        For Each sp As String In My.Computer.Ports.SerialPortNames
            ComLB.Items.Add(sp)
        Next

        ComLB.SelectedIndex = ComLB.FindString(My.Settings.Commport)

        If ComLB.SelectedIndex = -1 Then
            Connect.Enabled = False
            Discover.Enabled = False

        End If

        XBeeMy.Text = My.Settings.AtMY

        ' Lets try to reload the collection of destinations into the list

        Dim s As String
        Dim i As Int16

        For i = 0 To My.Settings.RobotList.Count - 1
            s = My.Settings.RobotList.Item(i)
            XBeeDL.Items.Add(s)
            Debug.Print(s)
        Next

        XBeeDL.Text = My.Settings.AtDL

        ' Initialize the state of our cached buttons.
        ButtonsState(0) = 0
        ButtonsState(1) = 0
        fDataHasChanged = False
        fNewPacketMsgSent = False
        fNewPacketMsgMode = False
        ' create our timer
        XBSW = New Stopwatch()
        RunningWithPipe = False
        g_fSendString = False
    End Sub

    Private Function APIRecvPacket(ByVal com1 As IO.Ports.SerialPort)
        Dim bPacketHeader(3) As Byte
        Dim wPacketSize As UInt16
        Dim bChksum As Byte
        Dim cbRead As Byte

        Try
            cbRead = 0
            Do
                cbRead += com1.Read(bPacketHeader, cbRead, 3 - cbRead)

            Loop Until cbRead = 3

            If bPacketHeader(0) <> &H7E Then
                Return vbNull
            End If
            wPacketSize = (bPacketHeader(1) << 8) + bPacketHeader(2)
            Dim bPacket(wPacketSize) As Byte

            cbRead = 0
            Do
                cbRead += com1.Read(bPacket, cbRead, wPacketSize - cbRead)
            Loop Until cbRead = wPacketSize
            bChksum = com1.ReadByte()

            ' validate checksum here!
            Dim wChksum As UInt16
            Dim i As UInt16
            wChksum = 0
            For i = 0 To wPacketSize - 1
                wChksum = wChksum + bPacket(i)
            Next
            wChksum = &HFF - (wChksum And &HFF)
            If bChksum = wChksum Then
                Return bPacket
            End If
            Debug.Print("API Recv Packet checksum error: " + bChksum.ToString + " <> " + wChksum.ToString)
        Catch ex As TimeoutException
            ' don't print stuff out for timeouts...
        Catch ex As Exception
            Debug.Print("API Recv Packet exception: " + ex.ToString)
        End Try
        Dim bEmptyPacket(0) As Byte
        Return bEmptyPacket

    End Function



    Private Function APISetLValue(ByVal com1 As IO.Ports.SerialPort, ByVal sName As String, ByVal lval As ULong)
        Dim bName() As Byte
        Dim bPacket(12) As Byte
        Dim oEncoder As New System.Text.ASCIIEncoding()

        Dim i As Byte


        Dim w As UInt16
        bName = oEncoder.GetBytes(sName)

        bPacket(0) = &H7E
        bPacket(1) = 0
        bPacket(2) = 8      ' this is the length LSB
        bPacket(3) = 8      ' CMD=8 which is AT command
        bPacket(4) = 42     ' could be anything here
        bPacket(5) = bName(0)
        bPacket(6) = bName(1)
        bPacket(7) = lval >> 24
        bPacket(8) = (lval >> 16) And &HFF
        bPacket(9) = (lval >> 8) And &HFF
        bPacket(10) = lval And &HFF
        w = 0
        For i = 3 To bPacket.Count - 2  ' don't include the frame delimter or length in count - likewise not the checksum itself...
            w = w + bPacket(i)
        Next
        bPacket(11) = &HFF - (w And &HFF)

        com1.Write(bPacket, 0, 12)
        com1.BaseStream.Flush()

        ' Now lets try to get a response!
        Try

            Dim bRecvPacket() As Byte
            com1.ReadTimeout = 100 'wait a maximum of .05 seconds for a response
            bRecvPacket = APIRecvPacket(com1)
            If (bRecvPacket.Count >= 5) Then
                If (bRecvPacket(0) = &H88) And (bRecvPacket(1) = bPacket(4)) And _
                        (bRecvPacket(2) = bName(0)) And (bRecvPacket(3) = bName(1)) And (bRecvPacket(4) = 0) Then
                    Return True
                End If
            End If

        Catch ex As Exception
            Debug.Print("API Set LValue: (" + sName + ") " + ex.ToString)
        End Try

        Return False
    End Function


    Private Sub APISendCmd(ByVal com1 As IO.Ports.SerialPort, ByVal sName As String)
        Dim bName() As Byte
        Dim bPacket(8) As Byte
        Dim oEncoder As New System.Text.ASCIIEncoding()

        Dim i As Byte


        Dim w As UInt16
        bName = oEncoder.GetBytes(sName)

        bPacket(0) = &H7E
        bPacket(1) = 0
        bPacket(2) = 4      ' this is the length LSB
        bPacket(3) = 8      ' CMD=8 which is AT command
        bPacket(4) = &H27   ' could be anything here
        bPacket(5) = bName(0)
        bPacket(6) = bName(1)
        w = 0
        For i = 3 To bPacket.Count - 2  ' don't include the frame delimter or length in count - likewise not the checksum itself...
            w = w + bPacket(i)
        Next
        bPacket(7) = &HFF - (w And &HFF)

        com1.Write(bPacket, 0, 8)
        com1.BaseStream.Flush()

    End Sub

    Private Function APIGetHVal(ByVal com1 As IO.Ports.SerialPort, ByVal sName As String)

        ' Output the request command
        Dim bName() As Byte
        Dim ulRetVal As UInt32
        Dim oEncoder As New System.Text.ASCIIEncoding()
        Dim fRepeat As Boolean
        bName = oEncoder.GetBytes(sName)

        APISendCmd(com1, sName)
        ulRetVal = &HFFFFFFF
        ' Now lets loop reading responses 
        ' Now lets try to get a response!
        fRepeat = True
        Do While fRepeat
            Try
                Dim bRecvPacket() As Byte
                com1.ReadTimeout = 100 'wait a maximum of .05 seconds for a response
                bRecvPacket = APIRecvPacket(com1)
                If (bRecvPacket.Count >= 5) Then
                    If (bRecvPacket(0) = &H88) And _
                            (bRecvPacket(2) = bName(0)) And (bRecvPacket(3) = bName(1)) And (bRecvPacket(4) = 0) Then
                        If bRecvPacket.Count <= 8 Then
                            ulRetVal = bRecvPacket(5)
                            ulRetVal = (ulRetVal << 8) + bRecvPacket(6)
                        Else
                            Return (bRecvPacket(5) << 24) + (bRecvPacket(6) << 16) + (bRecvPacket(7) << 8) + bRecvPacket(8)
                        End If
                        fRepeat = False
                    End If
                ElseIf bRecvPacket.Count <= 1 Then
                    fRepeat = False
                End If

            Catch ex As Exception
                Debug.Print("API Set LValue: (" + sName + ") " + ex.ToString)
                fRepeat = False
            End Try
        Loop
        Return ulRetVal

    End Function

    Private Function FConnectToComm()
        ' First thing we try to do is to initialize the XBEE with the appropriate baud rate, My String, etc
        Dim S As String
        Dim fInit As Boolean
        Dim fGotAnything As Boolean
        ' first go at high speed of 38400
        Try
            ' Was first see if we maybe have already initialized the xbee before - That is are we already
            ' configured to API mode.  We will start off by trying to set the MY using API and see if this works.  If so
            ' will continue to set the rest of stuff, else we will do what is necessary to convert to the right mode.
            ' will have changed...
            ' Remember the number we show is a hex number!
            fInit = False
            fGotAnything = False
            Try
                Dim sT As String = ExtractXbeeDL(XBeeDL.Text)
                XBeeCurDL = UInt16.Parse(sT, Globalization.NumberStyles.HexNumber)         ' ' save away the XBeedl...
            Catch ex As Exception
                XBeeCurDL = 0               ' none set for now...
                Debug.Print("Cur DL not set: " + ex.ToString)
            End Try

            Try
                Dim sT As String = ExtractXbeeDL(XBeeMy.Text)
                XBeeCurMy = UInt16.Parse(sT, Globalization.NumberStyles.HexNumber)         ' ' save away the XBeedl...
            Catch ex As Exception
                XBeeCurMy = 0               ' none set for now...
                Debug.Print("Cur My is invalid: " + ex.ToString)
            End Try

            Debug.Print("Try connect at 38400")
            XBeeCommport = ComLB.Items(ComLB.SelectedIndex)
            Using com1 As IO.Ports.SerialPort = _
                My.Computer.Ports.OpenSerialPort(XBeeCommport, 38400, IO.Ports.Parity.None)
                com1.Handshake = IO.Ports.Handshake.None
                System.Threading.Thread.Sleep(2000)
                ClearInputBuffer(com1)      ' make sure there is nothing there to start with...
                ' First lets try API mode...
                Debug.Print("My was: " + Hex(APIGetHVal(com1, "MY")))

                If APISetLValue(com1, "MY", XBeeCurMy) And APISetLValue(com1, "DL", XBeeCurDL) Then
                    Debug.Print("XBee is in API mode")
                    fInit = True
                Else
                    System.Threading.Thread.Sleep(2000)     'Not sure yet if we set the short time out ...
                    ClearInputBuffer(com1)      ' make sure there is nothing there to start with...
                    com1.NewLine = Chr(13)
                    com1.Write("+++")
                    com1.BaseStream.Flush()
                    com1.ReadTimeout = 2000 'wait a maximum of 2 seconds for a response
                    Try
                        S = com1.ReadLine()         ' should see what we got...
                        fGotAnything = True
                        Debug.Print("+++ returned something")
                    Catch ex As Exception
                    End Try
                    If fGotAnything Then
                        System.Threading.Thread.Sleep(20)
                        com1.Write("ATGT 3" + Chr(13))                ' set the guard time
                        com1.Write("ATMY " + XBeeMy.Text + Chr(13))   ' set the MY address
                        com1.Write("ATDL " + ExtractXbeeDL(XBeeDL.Text) + Chr(13))   ' set the DL address
                        com1.Write("ATAP 1" + Chr(13))                  ' go into API mode...
                        com1.Write("ATCN" + Chr(13))                  ' exit command mode
                        com1.BaseStream.Flush()
                        ' If we get an OK then we proably init before

                        '??? Not sure what to do here yet as I asked to go tinto API mode...
                        com1.ReadTimeout = 50 'wait a maximum of .05 seconds for a response
                        Try
                            S = com1.ReadLine()
                            If S.Length > 0 Then
                                fInit = True    ' hopefully we are now init
                            End If

                        Catch ex As Exception
                        End Try
                    End If
                End If
            End Using

            If Not fInit Then
                Debug.Print("Try connect at 9600")
                Using com1 As IO.Ports.SerialPort = _
                        My.Computer.Ports.OpenSerialPort(XBeeCommport, 9600, IO.Ports.Parity.None)
                    com1.Handshake = IO.Ports.Handshake.None
                    System.Threading.Thread.Sleep(2000)
                    com1.NewLine = Chr(13)
                    com1.Write("+++")
                    com1.BaseStream.Flush()

                    System.Threading.Thread.Sleep(2000)  ' have to wait for it to get into command mode...

                    ' now lets output all of the initial stuff.  We probably want to only do this once per time, but...
                    com1.Write("atni  PC DIY Remote" + Chr(13))   ' Set our title
                    com1.Write("ATGT 3" + Chr(13))                ' set the guard time
                    com1.Write("ATMY " + XBeeMy.Text + Chr(13))   ' set the MY address
                    com1.Write("ATDL " + ExtractXbeeDL(XBeeDL.Text) + Chr(13))   ' set the DL address
                    com1.Write("ATBD 5" + Chr(13))             ' set baud rate to 38400 see if we can do this on pc

                    com1.BaseStream.Flush()
                    System.Threading.Thread.Sleep(50)       ' have to wait for it to get into command mode...
                    ' Now we wish to read in all of the responses we got back
                    com1.ReadTimeout = 200 'wait a maximum of .2 seconds for a response
                    Try
ReadLoop:
                        S = com1.ReadLine()
                        If S.Length > 0 Then
                            LCDLB.Items.Add(S)
                            fInit = True    ' we got something...

                            GoTo ReadLoop
                        End If

                    Catch ex As Exception
                    End Try
                    LCDLB.TopIndex = LCDLB.Items.Count - 1
                    com1.Write("ATCN" + Chr(13))                  ' exit command mode
                    com1.BaseStream.Flush()
                    com1.BaudRate = 38400
                End Using
                If fInit Then

                    Debug.Print("Try converting to API mode")
                    Using com1 As IO.Ports.SerialPort = _
                            My.Computer.Ports.OpenSerialPort(XBeeCommport, 38400, IO.Ports.Parity.None)
                        com1.Handshake = IO.Ports.Handshake.None
                        System.Threading.Thread.Sleep(20)
                        com1.NewLine = Chr(13)
                        com1.Write("+++")
                        com1.BaseStream.Flush()

                        System.Threading.Thread.Sleep(20)  ' have to wait for it to get into command mode...
                        com1.Write("ATAP 1" + Chr(13))                  ' go into API mode...
                        com1.Write("ATCN" + Chr(13))                  ' exit command mode
                        com1.BaseStream.Flush()
                        ClearInputBuffer(com1)      ' make sure there is nothing there to start with...
                    End Using
                End If

            End If

            Return fInit
        Catch ex As Exception
            Beep()
        End Try
        Return False

    End Function



    Private Sub Connect_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Connect.Click
        If Connect.Text = "Connect" Then
            If FConnectToComm() Then
                Timer1.Enabled = True       ' also get our timer running...

                Connect.Text = "Disconnect"

                CommThread.RunWorkerAsync()
            End If
        Else
            Connect.Text = "Connect"
            CommThread.CancelAsync()            ' turn off the thread...
        End If

    End Sub


    Public Sub ClearInputBuffer(ByRef com1 As IO.Ports.SerialPort)
        Dim cbRead As Byte
        Dim b(1) As Byte

        com1.DiscardInBuffer()    ' flush out anything we have
        cbRead = 1
        com1.ReadTimeout = 100 'wait a maximum of .1 seconds for a response
        Try
            While cbRead > 0
                cbRead = com1.Read(b, 0, 1)
            End While

        Catch e As TimeoutException
            Debug.Print("TO: Clear Input Buffer")
        Catch ex As Exception

        End Try

    End Sub

    '==============================================================================
    ' [SendXBeePacket] function
    '   We will try to funnel all of our writes through this function such that
    '   we can experiment with change to binary...
    '  
    '==============================================================================

    Public Sub SendXBeePacket(ByRef com1 As IO.Ports.SerialPort, ByVal bPHType As Byte, ByVal cbExtra As Byte, ByRef pbextra As Byte())
        ' local variables
        ' BUGBUG:: First pass use old packet formats, so we will be replicating some fields like size and check sums
        ' Ok we are in API mode lets try to build the actual data packet...
        Dim wChkSum As UShort       ' unsigned
        Dim bDataPacket(cbExtra + XBEE_API_PH_SIZE + 9) As Byte

        ' The first 8 bytes are the API packet information
        bDataPacket(0) = &H7E               ' packet start delimiter
        bDataPacket(1) = 0                  ' Can send max 100 bytes data so MSB of size will be zero.
        bDataPacket(2) = 5 + 1 + cbExtra
        bDataPacket(3) = 1                  ' Send command 1 data with 16 bit DL.
        If bAPISeqNum = &HFF Then
            bAPISeqNum = 1                  ' don't overflow and don't use zero
        Else
            bAPISeqNum = bAPISeqNum + 1     ' increment to the next one
        End If
        bDataPacket(4) = bAPISeqNum
        bDataPacket(5) = XBeeCurDL >> 8
        bDataPacket(6) = XBeeCurDL And &HFF
        bDataPacket(7) = 0                  ' Options byte

        ' Now the data bytes...
        bDataPacket(8 + 0) = bPHType            ' Command type

        ' copy any extra data down
        If cbExtra Then
            For i = 0 To (cbExtra - 1)
                bDataPacket(8 + XBEE_API_PH_SIZE + i) = pbextra(i)
            Next
        End If

        ' now calculate the packets checksum...
        ' need to add in some of the actual packets data to the checksum
        wChkSum = 0
        For i = 3 To 7 + XBEE_API_PH_SIZE + cbExtra  ' don't include the delimiter or size fields...
            wChkSum += bDataPacket(i)
        Next
        bDataPacket(cbExtra + XBEE_API_PH_SIZE + 9 - 1) = &HFF - (wChkSum And &HFF)

        com1.Write(bDataPacket, 0, cbExtra + XBEE_API_PH_SIZE + 9)     ' Ok lets write out the data. XP 12 -> 14
        com1.BaseStream.Flush()                             ' Make sure it goes out now

    End Sub



    '==============================================================================
    ' [CheckAndTransmitDataPacket] function
    ' 
    ' This function will output a packet of data over the XBee to the receiving robot.  This function
    ' will start off simple and simply dump the raw data.  Then I will build in additional smarts.  Things like:
    ' only transmit data if it has changed.  Maybe allow the remote robot be able to send us information like
    ' how much slop should we allow in the deadband range for each channel.  Also Maybe we need to generate
    ' a pulse to allow the other side know that we are still there...
    '==============================================================================

    ''
    Public Sub CheckAndTransmitDataPacket(ByRef com1 As IO.Ports.SerialPort)
        '
        ' local variables
        Dim bDelim As Byte
        Dim wPacketSize As UInt16
        Dim bChksumRead As Byte
        Dim wChkSum As UShort       ' unsigned
        Dim cbRead As Byte
        Dim bDataOffset As Byte

        ' We will first check to see if there is any outstanding bytes waiting to be read.  If so we read in a complete 
        ' XBEE API packet, that we will then process.
        Try

            While com1.BytesToRead > 4 ' We have at least 7E <len1> <len2> <msgtype> <chksum>

                ' Read in an XBEE Packet.  The XBee packets are in the form:
                ' &h7e Size(word) databytes checksum
                com1.ReadTimeout = 100 'wait a maximum of .1 seconds for a response

                bDelim = com1.ReadByte()    ' read what should be a packet delimiter 
                If bDelim <> &H7E Then
                    Throw New ApplicationException("Not XBee Packet Delim")
                End If

                ' Now lets get the packet size
                wPacketSize = (com1.ReadByte() << 8) + com1.ReadByte()
                'Debug.Print("L: " + wPacketSize.ToString)
                Dim XBeePacket(wPacketSize) As Byte
                Dim wPacketDL As UInt16

                ' And get all of the bytes of the packet
                cbRead = 0
                Do
                    cbRead += com1.Read(XBeePacket, cbRead, wPacketSize - cbRead)
                Loop Until cbRead = wPacketSize

                ' finally get the checksum
                bChksumRead = com1.ReadByte()

                ' Now lets validate the checksum?  May not be necessary XBee probably did that earlier!
                wChkSum = 0
                For i = 0 To wPacketSize - 1
                    wChkSum = wChkSum + XBeePacket(i)
                Next
                If bChksumRead <> (&HFF - (wChkSum And &HFF)) Then
                    Throw New ApplicationException("Not XBee Checksum error")
                End If

                ' We appear to have a valid XBee packet, lets see what type it is.
                ' first see if it is a RX 16 bit or 64 bit packet?
                If XBeePacket(0) = &H81 Then
                    ' 16 bit address sent, so there is 5 bytes of packet header before our data
                    bDataOffset = 5
                    wPacketDL = XBeePacket(1)                       ' Get the 
                    wPacketDL = (wPacketDL << 8) + XBeePacket(2)
                ElseIf XBeePacket(0) = &H80 Then
                    ' 64 bit address so our data starts at offset 11...
                    bDataOffset = 11
                    ' Should save away the 64 bit address... 

                ElseIf XBeePacket(0) = &H89 Then
                    ' this is an A TX Status message - May check status and maybe update something?
                    bDataOffset = 0

                Else
                    Debug.Print("Unknown XBEE Packet: " + Hex(XBeePacket(0)) + " L:" + Hex(XBeePacket(1) >> 8) + Hex(XBeePacket(2)))
                    bDataOffset = 0
                End If

                ' Validate the packet. If A data request, it should be simple
                ' Simple request, no extra data, so checksum should equal the packet number...
                'Debug.Print("XBEE Packet: " + Hex(XBeePacket(0)) + " L:" + Hex(XBeePacket(1) >> 8) + Hex(XBeePacket(2)))
                If bDataOffset <> 0 Then
                    wPacketSize = wPacketSize - (bDataOffset + XBEE_API_PH_SIZE)    ' This is the extra data size

                    If (XBeePacket(bDataOffset + 0) = XBEE_RECV_DISP_VAL) Then
                        If (wPacketSize = 2) Then
                            DisplayRemoteValue(XBeePacket(bDataOffset + XBEE_API_PH_SIZE) << 8 + XBeePacket(bDataOffset + XBEE_API_PH_SIZE + 1))
                        End If
                    ElseIf (XBeePacket(bDataOffset + 0) = XBEE_RECV_DISP_STR) Then
                        If (wPacketSize > 0) Then
                            DisplayRemoteString(wPacketDL, XBeePacket, bDataOffset + XBEE_API_PH_SIZE, wPacketSize)
                        End If
                    ElseIf (XBeePacket(bDataOffset + 0) = XBEE_PLAY_SOUND) Then
                        PlayRemoteSounds(XBeePacket, bDataOffset + XBEE_API_PH_SIZE, wPacketSize)

                    Else
                        'We got something we were not expecting so error out.	
                        'System.Threading.Thread.Sleep(250)  ' Sleep for a bit to hopefully get the gunk out.
                        'ClearInputBuffer(com1)
                    End If
                End If
            End While

        Catch e As TimeoutException
            'Debug.Print("TO ???")
        Catch ex As Exception
            SyncLock SCListBox.SyncRoot
                SCListBox.Add("Check and Transmite exception: " + ex.ToString)
            End SyncLock
            CommThread.CancelAsync()            ' turn off the thread...
        End Try

        ' 
        If g_fSendString Then
            Try
                Dim bText() As Byte
                Dim oEncoder As New System.Text.ASCIIEncoding()

                bText = oEncoder.GetBytes(g_sUserInput)
                SendXBeePacket(com1, XBEE_DEBUG_STRING, bText.Length, bText)
                g_fSendString = False
            Catch ex As Exception
                SyncLock SCListBox.SyncRoot
                    SCListBox.Add("Transmite exception: " + ex.ToString)
                End SyncLock
            End Try
        End If


        ' we did not get anything or we did not get a complet packet...
        If XBSW.ElapsedMilliseconds() > 2000 Then
            'We got something we were not expecting so error out.	
            ClearInputBuffer(com1)
            XBeeTransDebugAttach(com1)            ' send out another ready signal.
        End If
        Return


    End Sub



    '==============================================================================
    ' [XBeeTransDebugAttach] - Simple message that we send out when we are ready
    '		or if we have not received anything for a long time...
    '
    '		This packet currently our MY that the destination will use to
    '		talk back to us, such that we can have multiple transmitters.
    '==============================================================================

    Public Sub XBeeTransDebugAttach(ByRef com1 As IO.Ports.SerialPort)
        Dim bMy(2) As Byte
        Dim wXbee As UInt16
        ' first need to get MY and convert to a number
        wXbee = XBeeCurMy  '
        bMy(0) = wXbee >> 8            ' high byte
        bMy(1) = wXbee And &HFF        ' low byte
        SendXBeePacket(com1, XBEE_DEBUG_ATTACH, 2, bMy)

        XBSW.Reset()                    ' reset our stop watch back to zero
        XBSW.Start()
        fNewPacketMsgSent = False       ' give it a chance to send again...
    End Sub


    '==============================================================================
    ' [XBeeTransDebugDetach] - Simple message that we send out when are no longer ready
    '		to receive anything.  Such as we are in some configuration menu...
    '		especially if we may change our MY setting...
    '
    '		This packet currently has no extra data.
    '==============================================================================

    Public Sub XBeeTransDebugDetach(ByRef com1 As IO.Ports.SerialPort)
        Dim b(1) As Byte
        SendXBeePacket(com1, XBEE_DEBUG_DETACH, 0, b)
    End Sub


    Public Sub AddItemToLB(ByVal text As String)
        LCDLB.Items.Add(text)
        LCDLB.TopIndex = LCDLB.Items.Count - 1
    End Sub

    '==============================================================================
    ' [DisplayRemoteString (pStr, cbOffset, cbStr)]
    ' 
    ' This function takes care of displaying string and or a number sent to us 
    ' rom the remote robot.  For now hard coded to a specific location on line 2...
    '
    ' This function will also switch to the appropriate display mode if necessary.
    '==============================================================================
    Public Sub DisplayRemoteString(ByVal wPacketDL As UInt16, ByRef pstr() As Byte, ByVal cbOffset As Byte, ByVal cbStr As Byte)
        ' Make sure we are in the right mode to display the data

        If cbStr Then
            'Dim s As String
            'Dim i As Byte
            's = Chr(pstr(cbOffset))
            'For i = cbOffset + 1 To cbOffset + cbStr - 1
            ' s = s + Chr(pstr(i))
            'Next
            Dim s As String = Mid(System.Text.Encoding.ASCII.GetString(pstr), cbOffset + 1, cbStr)

            ' now see if the string came from our current DL or from another source for us to display...
            If wPacketDL <> XBeeCurDL Then
                ' BugBug:: Build a list of names for robots to use to display
                s = Hex(wPacketDL) + ": " + s
            End If

            ' If LCDLB.InvokeRequired Then
            'Dim d As New SetTextCallback(AddressOf AddItemToLB)
            'Me.Invoke(d, New Object() {s})
            'Else
            'LCDLB.Items.Add(s)
            'LCDLB.TopIndex = LCDLB.Items.Count - 1
            'end if
            ' Add to our String collection.
            SyncLock SCListBox.SyncRoot
                SCListBox.Add(s)
            End SyncLock
        End If
        Return
    End Sub

    '==============================================================================
    ' [PlayRemoteSounds (pStr, cbOffset, cbStr)]
    '==============================================================================
    Public Sub PlayRemoteSounds(ByRef pstr() As Byte, ByVal cbOffset As Byte, ByVal cbStr As Byte)
        ' To save packet space the fequencies that were sent were divided by 25
        ' I was not hearing anything so multiply duration by 10...
        If cbStr Then
            Dim s As String
            Dim i As Byte
            s = Chr(pstr(cbOffset))
            For i = cbOffset + 1 To cbOffset + cbStr - 1 Step 2
                Dim freq As Long = pstr(cbOffset + 1) * 25
                Dim dur As Long = pstr(cbOffset) * 10
                System.Console.Beep(freq, dur)
            Next
        End If
        Return
    End Sub




    '==============================================================================
    ' [DisplayRemoteValue (num)]
    ' 
    ' This function takes care of displaying a number sent to us 
    ' rom the remote robot.  For now hard coded to a specific location on line 2...
    '
    ' This function will also switch to the appropriate display mode if necessary.
    '==============================================================================
    Public Sub DisplayRemoteValue(ByVal bVal As UInt16)
        ' Make sure we are in the right mode to display the data

        ' now display the value - For now one or the other.
        'gosub DoLCDDisplay[13, 2, 0, bVal]

        Return
    End Sub



    Private Sub CommThread_DoWork(ByVal sender As System.Object, ByVal e As System.ComponentModel.DoWorkEventArgs) Handles CommThread.DoWork

        CommThread.ReportProgress(0)
        XBSW.Start()
        'Dim Counter As Byte



        Using com1 As IO.Ports.SerialPort = _
            My.Computer.Ports.OpenSerialPort(XBeeCommport, 38400, IO.Ports.Parity.None)
            com1.Handshake = IO.Ports.Handshake.None
            com1.DtrEnable = False
            com1.NewLine = Chr(13)

            While Not CommThread.CancellationPending              ' turn off the thread...

                CheckAndTransmitDataPacket(com1)                ' just loop trying to send packets


            End While

            Try
                XBeeTransDebugDetach(com1)            ' send out a not ready in case it was waiting for us
            Catch ex As Exception
            End Try

        End Using


        CommThread.ReportProgress(99)

    End Sub


    Private Sub CommThread_ProgressChanged(ByVal sender As Object, ByVal e As System.ComponentModel.ProgressChangedEventArgs) Handles CommThread.ProgressChanged
        If e.ProgressPercentage < 10 Then
            LCDLB.Items.Add("*** Thread Start ***")
            LCDLB.TopIndex = LCDLB.Items.Count - 1
        ElseIf e.ProgressPercentage > 90 Then
            LCDLB.Items.Add("*** Thread Canceled ***")
            LCDLB.TopIndex = LCDLB.Items.Count - 1
            Timer1.Enabled = False  'We can stop our timer now...
            Connect.Text = "Connect"
        End If
    End Sub

    
    Private Sub ComLB_DropDown(sender As Object, e As System.EventArgs) Handles ComLB.DropDown
        If ComLB.SelectedIndex = -1 Then
            ComLB.Items.Clear() ' first clear our list.
            For Each sp As String In My.Computer.Ports.SerialPortNames
                ComLB.Items.Add(sp)
            Next

            ComLB.SelectedIndex = ComLB.FindString(My.Settings.Commport)

            Connect.Enabled = False
            Discover.Enabled = False
        End If


    End Sub

    Private Sub ComLB_SelectedIndexChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles ComLB.SelectedIndexChanged
        ' Show all available COM ports.

        If (ComLB.SelectedIndex >= 0) Then
            My.Settings.Commport = ComLB.Items(ComLB.SelectedIndex).ToString
            Connect.Enabled = True
            Discover.Enabled = True
        Else
            ' nothing selected, disable a few controls.
            Connect.Enabled = False
            Discover.Enabled = False
        End If

    End Sub

    Private Sub XBeeMy_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles XBeeMy.TextChanged
        My.Settings.AtMY = XBeeMy.Text
    End Sub


    Private Function ExtractXbeeDL(ByRef s As String)
        ' This function will look through the string that is shown and only return back the part at the beginning that is a hex number.  This
        ' is needed as we now popuplate a drop down list with the robot names we discovered.
        Dim i As Short
        Dim c As Char
        If s.Length = 0 Then
            Return String.Empty
        End If
        For i = 0 To (s.Length - 1)
            c = s.Substring(i, 1)
            If (c >= "0" And c <= "9") Or (c >= "a" And c <= "f") Or (c >= "A" And c <= "F") Then
                ' valid char should turn this around
            Else
                If i <> 0 Then
                    Return s.Substring(0, i)
                Else
                    Return String.Empty
                End If
            End If
        Next
        Return s ' whole string is valid...
    End Function

    Private Sub TBTerminal_KeyPress(ByVal sender As Object, ByVal e As System.Windows.Forms.KeyPressEventArgs) Handles TBTerminal.KeyPress
        If e.KeyChar = ControlChars.Cr Then
            g_sUserInput = TBTerminal.Text + ControlChars.CrLf

            g_fSendString = True
            LCDLB.Items.Add(">> " + g_sUserInput)
            LCDLB.TopIndex = LCDLB.Items.Count - 1
            TBTerminal.Text = ""
            e.Handled = True
        End If
    End Sub

    Private Sub XBeeDL_MouseDown(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles XBeeDL.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Right Then
            XBeeDL.ContextMenuStrip = XBeeDLContextMenu

        End If
    End Sub


    Private Sub XBDLCM_DeleteItem_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles XBDLCM_DeleteItem.Click
        If XBeeDL.SelectedIndex <> -1 Then
            ' First remove from our save list...
            My.Settings.RobotList.Remove(XBeeDL.Text)
            XBeeDL.Items.RemoveAt(XBeeDL.SelectedIndex)
        End If
    End Sub

    Private Sub XBDLCM_Clear_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles XBDLCM_Clear.Click
        My.Settings.RobotList.Clear()
        XBeeDL.Items.Clear()
        XBeeDL.Text = ""
    End Sub


    Private Sub XBeeDL_TextChanged(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles XBeeDL.TextChanged
        My.Settings.AtDL = XBeeDL.Text
    End Sub


    Private Sub Discover_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles Discover.Click
        LCDLB.Items.Add("Start Robot Search")
        LCDLB.TopIndex = LCDLB.Items.Count - 1

        If FConnectToComm() Then

            Using com1 As IO.Ports.SerialPort = _
                    My.Computer.Ports.OpenSerialPort(XBeeCommport, 38400, IO.Ports.Parity.None)
                APISendCmd(com1, "ND")          ' send out the ATND command.

                ' Now we wish to loop through and see if we get any responses.
                com1.ReadTimeout = 3000 'wait a maximum of 3 seconds for a response
                Dim bRecvPacket() As Byte
                Do
                    bRecvPacket = APIRecvPacket(com1)       ' try to retrieve the next packet of data
                    If bRecvPacket.Count < 12 Then
                        Exit Do
                    End If
                    Dim wMy As UInt16
                    Dim lSNH As UInt32
                    Dim lSNL As UInt32
                    Dim i As Byte
                    'shift operators appear to have problems?
                    wMy = 0
                    For i = XBEE_ATRESP_DATA_OFFSET + 0 To XBEE_ATRESP_DATA_OFFSET + 1
                        wMy = (wMy << 8) + bRecvPacket(i)
                    Next

                    lSNH = 0
                    For i = XBEE_ATRESP_DATA_OFFSET + 2 To XBEE_ATRESP_DATA_OFFSET + 5
                        lSNH = (lSNH << 8) + bRecvPacket(i)
                    Next

                    lSNL = 0
                    For i = XBEE_ATRESP_DATA_OFFSET + 6 To XBEE_ATRESP_DATA_OFFSET + 9
                        lSNL = (lSNL << 8) + bRecvPacket(i)
                    Next

                    Dim s As String = Hex(wMy) + " - " + Mid(System.Text.Encoding.ASCII.GetString(bRecvPacket), XBEE_ATRESP_DATA_OFFSET + 12, _
                                    bRecvPacket.Count - (XBEE_ATRESP_DATA_OFFSET + 13)) + " (" + Hex(lSNH) + " " + Hex(lSNL) + ") "

                    ' Before we add this item, we should see if we already have an exact match.  If so don't add again.  If not, we should
                    ' then check to see if we already have the serial number in our list.  If so the user may have changed the MY or the
                    ' NI of the item so update that in our list...
                    If XBeeDL.FindString(s) <> -1 Then
                        LCDLB.Items.Add("Duplicate Found: " + s)
                        ' Duplicate found
                    Else
                        Dim isn As Integer = InStr(s, "(")
                        Dim fFound As Boolean
                        fFound = False
                        If XBeeDL.Items.Count > 0 Then
                            For i = 0 To XBeeDL.Items.Count - 1
                                Dim sItem As String = XBeeDL.Items.Item(i)
                                Dim iSNItem As Integer = InStr(sItem, "(")

                                If (isn > 0) And (iSNItem > 0) And (Mid(s, isn) = Mid(sItem, iSNItem)) Then
                                    LCDLB.Items.Add("Item Updated: " + s)
                                    XBeeDL.Items.Item(i) = s
                                    My.Settings.RobotList.Item(i) = s
                                    fFound = True
                                    Exit For

                                End If

                            Next

                        End If
                        If Not fFound Then
                            XBeeDL.Items.Add(s)
                            LCDLB.Items.Add("Found: " + s)
                            My.Settings.RobotList.Add(s)
                        End If

                    End If
                Loop While True

            End Using
            LCDLB.Items.Add("Robot Search - Completed")
            LCDLB.TopIndex = LCDLB.Items.Count - 1


        End If
    End Sub
    '=======================================================================================================
    'Pipe plugin
    ' read in an integer from the pipe. Since we read in bytes we need to change those bytes to
    ' their integer counterparts.
    Private Function ReadInteger(ByRef hPipe)

        Dim bytesRead As Integer
        Dim data As Byte() = {0, 0, 0, 0}
        bytesRead = 4

        ReadFile(hPipe, data(0), 4, bytesRead, Nothing)

        ReadInteger = data(0) Or (data(1) * 256) Or (data(2) * 65536) Or (data(3) * 16777216)

    End Function


    ' returns an error message to RR. This message is displayed in the "messages" list within
    ' the RR Pipe Program interface.
    Private Sub ReturnError(ByVal hPipe As Long, ByRef txt As String)

        Dim bytesWrite As Long
        Dim writeLen As Integer
        Dim txtBytes As Byte()
        Dim dataLen As Integer

        writeLen = 5
        ' write the message name
        WriteFile(hPipe, intToByte(writeLen), 4, bytesWrite, Nothing)
        WriteFile(hPipe, ASCII.GetBytes("error"), writeLen, bytesWrite, Nothing)
        ' write the message data
        dataLen = Len(txt)
        txtBytes = ASCII.GetBytes(txt)
        WriteFile(hPipe, intToByte(dataLen), 4, bytesWrite, Nothing)
        WriteFile(hPipe, txtBytes, dataLen, bytesWrite, Nothing)

    End Sub

    ' returns a byte string variable to RR. The returned variables can be used
    ' elsewhere in RR for continued processing.
    Private Sub ReturnBytesVariable(ByVal hPipe As Long, ByRef name As String, ByRef data As Byte(), ByVal dataLen As Long)

        Dim bytesWrite As Long
        Dim writeLen As Integer
        writeLen = Len(name)
        Dim varData As Byte()

        varData = ASCII.GetBytes(name)

        ' write the message name
        WriteFile(hPipe, intToByte(writeLen), 4, bytesWrite, Nothing)
        WriteFile(hPipe, varData, writeLen, bytesWrite, Nothing)
        ' write the message data
        WriteFile(hPipe, intToByte(dataLen), 4, bytesWrite, Nothing)
        If dataLen > 0 Then
            WriteFile(hPipe, data, dataLen, bytesWrite, Nothing)
        End If

    End Sub

    Private Sub ReturnIntVariable(ByVal hPipe As Long, ByRef name As String, ByVal data As Long)

        Dim bytesWrite As Long
        Dim dataLen As Integer
        Dim dataBytes As Byte()
        Dim dataText As String

        Dim varData As Byte()

        varData = ASCII.GetBytes(name)

        dataText = CStr(data)
        dataBytes = ASCII.GetBytes(dataText)

        dataLen = Len(name)
        ' write the message name
        WriteFile(hPipe, intToByte(dataLen), 4, bytesWrite, Nothing)
        WriteFile(hPipe, varData, dataLen, bytesWrite, Nothing)
        ' write the message data
        dataLen = Len(dataText)
        WriteFile(hPipe, intToByte(dataLen), 4, bytesWrite, Nothing)
        WriteFile(hPipe, dataBytes, dataLen, bytesWrite, Nothing)

    End Sub

    



    Private Sub LCDLB_MouseDown1(ByVal sender As Object, ByVal e As System.Windows.Forms.MouseEventArgs) Handles LCDLB.MouseDown
        If e.Button = Windows.Forms.MouseButtons.Right Then
            LCDLB.ContextMenuStrip = PMTerminal
        End If

    End Sub

    Private Sub LCDLBClear_Click(ByVal sender As Object, ByVal e As System.EventArgs) Handles LBTerminalClear.Click
        LCDLB.Items.Clear() ' clear out the listbox
    End Sub

    Private Sub LBTerminalSelectAll_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LBTerminalSelectAll.Click
        Dim i As Integer
        For i = 0 To LCDLB.Items.Count - 1
            LCDLB.SetSelected(i, True)
        Next

    End Sub

    Private Sub LBTerminalCopytoClipBoard_Click(ByVal sender As System.Object, ByVal e As System.EventArgs) Handles LBTerminalCopytoClipBoard.Click
        Dim i As Integer
        Dim s As String = ""
        For i = 0 To LCDLB.Items.Count - 1
            If LCDLB.GetSelected(0) Then
                s = s + LCDLB.Items.Item(i).ToString + ControlChars.CrLf
            End If
        Next
        If s <> "" Then
            Clipboard.SetDataObject(s)
        End If

    End Sub

    Private Sub Timer1_Tick(sender As System.Object, e As System.EventArgs) Handles Timer1.Tick
        If SCListBox.Count > 0 Then
            LCDLB.BeginUpdate()
            SyncLock SCListBox.SyncRoot
                While SCListBox.Count > 0
                    LCDLB.Items.Add(SCListBox(0))           'get items from the start and add to our listbox
                    SCListBox.RemoveAt(0)                   'Delete that item from our list
                End While
            End SyncLock
            LCDLB.TopIndex = LCDLB.Items.Count - 1  ' And Scroll to that location...
            LCDLB.EndUpdate()
        End If
    End Sub

    Private Sub ResetXBee_Click(sender As System.Object, e As System.EventArgs) Handles ResetXBee.Click
        Dim s As String
        If FConnectToComm() Then
            ' We first connect to the comm, then we convert it back from API mode back to text replacement mode.
            If (CommThread.IsBusy) Then
                CommThread.CancelAsync()            ' turn off the thread...
            End If
            Using com1 As IO.Ports.SerialPort = _
                My.Computer.Ports.OpenSerialPort(XBeeCommport, 38400, IO.Ports.Parity.None)
                com1.Handshake = IO.Ports.Handshake.None
                System.Threading.Thread.Sleep(500)
                ClearInputBuffer(com1)      ' make sure there is nothing there to start with...

                ' Assume we are IN API mode
                LCDLB.Items.Add("My: " + Hex(APIGetHVal(com1, "MY")))

                ' Now lets get out of API mode
                APISetLValue(com1, "GT", 1000)  ' Guard time back to default
                APISetLValue(com1, "AP", 0)    ' Get out of API mode
                APISetLValue(com1, "DL", XBeeCurDL) ' Set the DL to the selected one in the list
                ClearInputBuffer(com1)      ' make sure there is nothing there to start with...

                ' Lets try to see if we succeeded or not...
                System.Threading.Thread.Sleep(1500)
                com1.NewLine = Chr(13)
                com1.Write("+++")
                com1.BaseStream.Flush()
                System.Threading.Thread.Sleep(1000)
                com1.ReadTimeout = 2000 'wait a maximum of 2 seconds for a response
                Try
                    s = com1.ReadLine()         ' should see what we got...
                    Debug.Print("+++ returned something")
                    com1.Write("ATMY" + Chr(13))   ' Get the MY address
                    com1.Write("ATDL" + Chr(13))   ' Get the MY address
                    com1.Write("ATCN" + Chr(13))                  ' exit command mode
                    s = com1.ReadLine()         ' should see what we got...
                    LCDLB.Items.Add("MY: " + s)
                    s = com1.ReadLine()         ' should see what we got...
                    LCDLB.Items.Add("DL: " + s)
                Catch ex As Exception
                    LCDLB.Items.Add(ex.ToString)
                End Try
                ClearInputBuffer(com1)      ' make sure there is nothing there to start with...
                LCDLB.Items.Add("Communications back in Text Mode")
            End Using
        End If

    End Sub
End Class


