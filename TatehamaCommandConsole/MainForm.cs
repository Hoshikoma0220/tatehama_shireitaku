using Dapplo.Microsoft.Extensions.Hosting.WinForms;
using OpenIddict.Client;
using System;
using System.Drawing;
using System.Windows.Forms;
using TatehamaCommandConsole.Communications;
using TatehamaCommandConsole.Manager;
using TatehamaCommandConsole.Models;
using TatehamaCommandConsole.Services;

namespace TatehamaCommandConsole
{
    public partial class MainForm : Form, IWinFormsShell
    {
        private readonly ServerCommunication _serverCommunication;          // �T�[�o�[�ʐM
        private readonly TrainCrewCommunication _trainCrewCommunication;    // TrainCrew�ʐM
        private readonly DataManager _dataManager;                          // GlobalData�Ǘ�
        private readonly Timer _mainTimer;                                  // ���C���^�C�}�[
        private TrainCrewStateData _trainCrewStateData;                     // TrainCrew�f�[�^
        private KokuchiForm _kokuchiForm;                                   // �^�]���m��t�H�[��
        private AccidentForm _accidentForm;                                 // �^�]�x��t�H�[��
        private TrackCircuitForm _trackCircuitForm;                         // �O����H�t�H�[��

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public MainForm(OpenIddictClientService openIddictClientService, ServerCommunication serverCommunication)
        {
            InitializeComponent();

            // �C���X�^���X����
            _serverCommunication = serverCommunication;
            _trainCrewCommunication = new TrainCrewCommunication();
            _dataManager = DataManager.Instance;
            // Form����
            _kokuchiForm = new KokuchiForm();
            _accidentForm = new AccidentForm();
            _trackCircuitForm = new TrackCircuitForm(serverCommunication);

            // �C�x���g�ݒ�
            Load += MainForm_Load;
            FormClosing += MainForm_FormClosing;
            _trainCrewCommunication.TrainCrewStateDataUpdated += UpdateTrainCrewStateData;

            // Timer�ݒ�
            _mainTimer = new();
            _mainTimer = InitializeTimer(100, MainTimer_Tick);

            var tsvFolderPath = "TSV";
            // �w�ݒ�f�[�^�����X�g�Ɋi�[
            _dataManager.StationSettingList = StationSettingLoader.LoadSettings(tsvFolderPath, "StationSettingList.tsv");

            // TrainCrew������
            TrainCrew.TrainCrewInput.Init();
        }

        /// <summary>
        /// MainForm_Load�C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainForm_Load(object sender, EventArgs e)
        {
            // ���[�U�[�F�؁E������
            await _serverCommunication.AuthenticateAsync();
            // TrainCrew�ڑ�
            _trainCrewCommunication.Command = "DataRequest";
            _trainCrewCommunication.Request = new[] { "all" };
            await _trainCrewCommunication.TryConnectWebSocket();
        }

        /// <summary>
        /// TrainCrewData����(�VAPI)
        /// </summary>
        private void UpdateTrainCrewStateData(TrainCrewStateData Data)
        {
            _trainCrewStateData = Data;
        }

        /// <summary>
        /// MainTimer_Tick�C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainTimer_Tick(object sender, EventArgs e)
        {
            // �T�[�o�[�ڑ���Ԃ̕\�����X�V
            UpdateServerConnectionState();

            // TrainCrew�f�[�^�擾(��API)
            var state = TrainCrew.TrainCrewInput.GetTrainState();
            TrainCrew.TrainCrewInput.RequestStaData();
            TrainCrew.TrainCrewInput.RequestData(TrainCrew.DataRequest.Signal);
            if (state == null || state.CarStates.Count == 0 || state.stationList.Count == 0) { return; }
            try { var dataCheck = state.stationList[state.nowStaIndex].Name; }
            catch { return; }

            //�^�]��ʑJ�ڂȂ珈��
            if (TrainCrew.TrainCrewInput.gameState.gameScreen == TrainCrew.GameScreen.MainGame
                || TrainCrew.TrainCrewInput.gameState.gameScreen == TrainCrew.GameScreen.MainGame_Pause
                || TrainCrew.TrainCrewInput.gameState.gameScreen == TrainCrew.GameScreen.MainGame_Loading)
            {
                SuspendLayout();



                ResumeLayout();
            }
        }

        /// <summary>
        /// MainForm_FormClosing�C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // �T�[�o�[�ؒf
            await _serverCommunication.DisconnectAsync();
            // �I������
            TrainCrew.TrainCrewInput.Dispose();
        }

        /// <summary>
        /// �őO�ʕ\���ؑփC�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Main_CheckBox_TopMost_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = Main_CheckBox_TopMost.Checked;
        }

        /// <summary>
        /// Button�N���b�N�C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void ButtonClickEvent(object sender, EventArgs e)
        {
            if (sender is Button button)
            {
                switch (button.Name)
                {
                    // �^�]���m��
                    case "Button_Select_Kokuchi":
                        {
                            if (_kokuchiForm.IsDisposed)
                            {
                                _kokuchiForm = new KokuchiForm();
                            }
                            _kokuchiForm.Show();
                        }
                        break;
                    // �^�]�x��
                    case "Button_Select_Accident":
                        {
                            if (_accidentForm.IsDisposed)
                            {
                                _accidentForm = new AccidentForm();
                            }
                            _accidentForm.Show();
                        }
                        break;
                    // �O����H
                    case "Button_Select_TrackCircuit":
                        {
                            if (_trackCircuitForm.IsDisposed)
                            {
                                _trackCircuitForm = new TrackCircuitForm(_serverCommunication);
                            }
                            _trackCircuitForm.Show();
                        }
                        break;
                    default:
                        break;
                }
            }
        }

        /// <summary>
        /// �T�[�o�[�ڑ���Ԃ̕\�����X�V
        /// </summary>
        private void UpdateServerConnectionState()
        {
            // �T�[�o�[�ڑ���Ԃ̕\�����X�V
            if (_dataManager.ServerConnected && (Label_ServerConectionState.Text != "�I�����C��"))
            {
                Label_ServerConectionState.Text = "�I�����C��";
                Label_ServerConectionState.ForeColor = ColorTranslator.FromHtml("#FF000000");
                Label_ServerConectionState.BackColor = ColorTranslator.FromHtml("#FF67FF4C");
            }
            else if (!_dataManager.ServerConnected && (Label_ServerConectionState.Text != "�I�t���C��"))
            {
                Label_ServerConectionState.Text = "�I�t���C��";
                Label_ServerConectionState.ForeColor = ColorTranslator.FromHtml("#FF888888");
                Label_ServerConectionState.BackColor = ColorTranslator.FromHtml("#FF555555");
            }
        }

        /// <summary>
        /// Timer���������\�b�h
        /// </summary>
        /// <param name="interval"></param>
        /// <param name="tickEvent"></param>
        /// <returns></returns>
        private static Timer InitializeTimer(int interval, EventHandler tickEvent)
        {
            var timer = new Timer
            {
                Interval = interval
            };
            timer.Tick += tickEvent;
            timer.Start();
            return timer;
        }
    }
}
