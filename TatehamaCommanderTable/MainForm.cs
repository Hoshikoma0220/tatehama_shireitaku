using Dapplo.Microsoft.Extensions.Hosting.WinForms;
using OpenIddict.Client;
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Forms;
using TatehamaCommanderTable.Communications;
using TatehamaCommanderTable.Manager;
using TatehamaCommanderTable.Services;

namespace TatehamaCommanderTable
{
    public partial class MainForm : Form, IWinFormsShell
    {
        private readonly DataManager _dataManager;
        private readonly ServerCommunication _serverCommunication;
        
        private KokuchiForm _kokuchiForm;
        private TroubleForm _accidentForm;
        private TrackCircuitForm _trackCircuitForm;

        private readonly Timer _mainTimer;

        /// <summary>
        /// �R���X�g���N�^
        /// </summary>
        public MainForm(OpenIddictClientService openIddictClientService, ServerCommunication serverCommunication)
        {
            InitializeComponent();

            // �C���X�^���X����
            _serverCommunication = serverCommunication;
            _dataManager = DataManager.Instance;

            // �w�ݒ�f�[�^�����X�g�Ɋi�[
            var tsvFolderPath = "TSV";
            _dataManager.StationSettingList = StationSettingLoader.LoadSettings(tsvFolderPath, "StationSettingList.tsv");

            // Form����
            _kokuchiForm = new KokuchiForm(serverCommunication);
            _accidentForm = new TroubleForm(serverCommunication);
            _trackCircuitForm = new TrackCircuitForm(serverCommunication);

            // �C�x���g�ݒ�
            Load += MainForm_Load;
            FormClosing += MainForm_FormClosing;

            // Timer�ݒ�
            _mainTimer = new();
            _mainTimer.Interval = 100;
            _mainTimer.Tick += MainTimer_Tick;
            _mainTimer.Start();
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
        }

        /// <summary>
        /// MainForm_FormClosing�C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // ����m�F�_�C�A���O�̏���
            if (!ConfirmClose())
            {
                e.Cancel = true;
                return;
            }
            // �T�[�o�[�ؒf
            await _serverCommunication.DisconnectAsync();
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
                                _kokuchiForm = new KokuchiForm(_serverCommunication);
                            }
                            _kokuchiForm.Show();
                        }
                        break;
                    // �^�]�x��
                    case "Button_Select_Accident":
                        {
                            if (_accidentForm.IsDisposed)
                            {
                                _accidentForm = new TroubleForm(_serverCommunication);
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
        /// MainTimer_Tick�C�x���g
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainTimer_Tick(object sender, EventArgs e)
        {
            // �T�[�o�[�ڑ���Ԃ̕\�����X�V
            UpdateServerConnectionState();
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
        /// �E�B���h�E�����ۂ̊m�F����
        /// </summary>
        /// <returns>�E�B���h�E����ėǂ��ꍇ��true�A����ȊO��false</returns>
        public bool ConfirmClose()
        {
            var result = CustomMessage.Show("�S�Ă̎i�ߑ��ʂ���܂��B��낵���ł����H",
                "�I���m�F",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.No)
            {
                return false;
            }

            // �^�C�}�[��~
            _mainTimer.Stop();

            return true;
        }
    }
}
