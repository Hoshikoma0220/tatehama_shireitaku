﻿using System;
using System.Windows.Forms;
using TatehamaCommandConsole.Communications;
using TatehamaCommandConsole.Manager;
using TatehamaCommandConsole.Models;

namespace TatehamaCommandConsole
{
    public partial class TrackCircuitForm : Form
    {
        private readonly DataManager _dataManager;
        private readonly ServerCommunication _serverCommunication;
        public event Action UpdateTrackCircuitData;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        /// <param name="serverCommunication"></param>
        public TrackCircuitForm(ServerCommunication serverCommunication)
        {
            InitializeComponent();

            // インスタンス取得
            _dataManager = DataManager.Instance;
            _serverCommunication = serverCommunication;

            // イベントハンドラ設定
            _serverCommunication.DataGridViewUpdated += (newDataSource) => UpdateDataSource(newDataSource);

            // DataGridViewのデータバインド
            TrackCircuit_BindingSource.DataSource = _dataManager.DataGridViewSettingList;

            // DataGridViewの設定
            SetupDataGridView();
        }

        /// <summary>
        /// 最前面表示切替イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TrackCircuit_CheckBox_TopMost_CheckedChanged(object sender, EventArgs e)
        {
            this.TopMost = TrackCircuit_CheckBox_TopMost.Checked;
        }

        /// <summary>
        /// DataGridView更新処理
        /// </summary>
        /// <param name="newDataSource"></param>
        public void UpdateDataSource(SortableBindingList<DataGridViewSetting> newDataSource)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => TrackCircuit_BindingSource.DataSource = newDataSource));
            }
            else
            {
                TrackCircuit_BindingSource.DataSource = newDataSource;
            }
        }

        /// <summary>
        /// DataGridViewの設定
        /// </summary>
        private void SetupDataGridView()
        {
            // 複数選択不可
            TrackCircuit_DataGridView_TrackCircuitData.MultiSelect = false;

            // 中央揃え
            TrackCircuit_DataGridView_TrackCircuitData.Columns["trainNumber"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            TrackCircuit_DataGridView_TrackCircuitData.Columns["shortCircuitStatus"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
            TrackCircuit_DataGridView_TrackCircuitData.Columns["lockingStatus"].DefaultCellStyle.Alignment = DataGridViewContentAlignment.MiddleCenter;
        }

        /// <summary>
        /// 選択したデータを表示
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void DataGridView_TrackCircuitData_SelectionChanged(object sender, EventArgs e)
        {
            if (TrackCircuit_DataGridView_TrackCircuitData.SelectedRows.Count > 0)
            {
                var selectedRow = TrackCircuit_DataGridView_TrackCircuitData.SelectedRows[0];
                string trackCircuit = selectedRow.Cells["trackCircuit"].Value.ToString();
                string trainNumber = selectedRow.Cells["trainNumber"].Value.ToString();
                string shortCircuitStatus = selectedRow.Cells["shortCircuitStatus"].Value.ToString();
                string lockingStatus = selectedRow.Cells["lockingStatus"].Value.ToString();

                // 各コントロールに設定
                TrackCircuit_Label_TrackCircuit.Text = trackCircuit;
                TrackCircuit_TextBox_TrainNumber.Text = trainNumber;
                TrackCircuit_RadioButton_ShortCircuit_ON.Checked = shortCircuitStatus == "〇";
                TrackCircuit_RadioButton_ShortCircuit_OFF.Checked = shortCircuitStatus != "〇";
                TrackCircuit_RadioButton_Locking_ON.Checked = lockingStatus == "〇";
                TrackCircuit_RadioButton_Locking_OFF.Checked = lockingStatus != "〇";
            }
        }
    }
}
