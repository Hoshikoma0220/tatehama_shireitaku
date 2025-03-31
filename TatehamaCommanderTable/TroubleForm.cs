﻿using System;
using System.Windows.Forms;
using TatehamaCommanderTable.Communications;
using TatehamaCommanderTable.Manager;
using TatehamaCommanderTable.Models;
using TatehamaCommanderTable.Services;

namespace TatehamaCommanderTable
{
    public partial class TroubleForm : Form
    {
        private readonly DataManager _dataManager;
        private readonly ServerCommunication _serverCommunication;

        /// <summary>
        /// コンストラクタ
        /// </summary>
        public TroubleForm(ServerCommunication serverCommunication)
        {
            InitializeComponent();

            // インスタンス取得
            _dataManager = DataManager.Instance;
            _serverCommunication = serverCommunication;

            // イベント設定
            Load += TroubleForm_Load;
            FormClosing += TroubleForm_FormClosing;
        }

        /// <summary>
        /// TroubleForm_Loadイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TroubleForm_Load(object sender, EventArgs e)
        {
            // イベントハンドラ設定
            _serverCommunication.TroubleDataGridViewUpdated += (newDataSource) => UpdateDataSource(newDataSource);

            // DataGridViewのデータバインド
            Trouble_BindingSource.DataSource = _dataManager.TroubleDataGridViewSettingList;

            // DataGridViewの設定
            SetupDataGridView();
        }

        /// <summary>
        /// TroubleForm_FormClosingイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TroubleForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            Hide();
            e.Cancel = true;
        }

        /// <summary>
        /// 最前面表示切替イベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Trouble_CheckBox_TopMost_CheckedChanged(object sender, System.EventArgs e)
        {
            this.TopMost = Trouble_CheckBox_TopMost.Checked;
        }

        /// <summary>
        /// ボタンクリックイベント
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void Trouble_Button_Click(object sender, EventArgs e)
        {
            try
            {
                Button button = sender as Button;
                switch (button.Name)
                {
                    // 削除ボタン
                    case "Trouble_Button_Cansel":
                        {

                        }
                        break;
                    // 設定ボタン
                    case "Trouble_Button_Set":
                        {

                        }
                        break;
                }
            }
            catch (Exception ex)
            {
                CustomMessage.Show(ex.ToString(), "エラー");
            }
        }

        /// <summary>
        /// DataGridView更新処理
        /// </summary>
        /// <param name="newDataSource"></param>
        public void UpdateDataSource(SortableBindingList<TroubleDataGridViewSetting> newDataSource)
        {
            try
            {
                if (!this.IsDisposed)
                {
                    if (this.IsHandleCreated && !this.IsDisposed)
                    {
                        // スクロール位置を保持
                        int firstDisplayedScrollingRowIndex = Trouble_DataGridView_SettingList.FirstDisplayedScrollingRowIndex;
                        int selectedRowIndex = Trouble_DataGridView_SettingList.CurrentCell?.RowIndex ?? 0;
                        if (firstDisplayedScrollingRowIndex < 0)
                        {
                            firstDisplayedScrollingRowIndex = 0;
                        }

                        // データバインド
                        if (this.InvokeRequired)
                        {
                            this.Invoke(new Action(() =>
                            {
                                if (!this.IsDisposed)
                                {
                                    Trouble_BindingSource.DataSource = newDataSource;
                                    if (Trouble_DataGridView_SettingList.Rows.Count > 0)
                                    {
                                        Trouble_DataGridView_SettingList.FirstDisplayedScrollingRowIndex = Math.Min(firstDisplayedScrollingRowIndex, Trouble_DataGridView_SettingList.Rows.Count - 1);
                                        Trouble_DataGridView_SettingList.CurrentCell = Trouble_DataGridView_SettingList.Rows[Math.Min(selectedRowIndex, Trouble_DataGridView_SettingList.Rows.Count - 1)].Cells[0];
                                    }
                                }
                            }));
                        }
                        else
                        {
                            if (!this.IsDisposed)
                            {
                                Trouble_BindingSource.DataSource = newDataSource;
                                if (Trouble_DataGridView_SettingList.Rows.Count > 0)
                                {
                                    Trouble_DataGridView_SettingList.FirstDisplayedScrollingRowIndex = Math.Min(firstDisplayedScrollingRowIndex, Trouble_DataGridView_SettingList.Rows.Count - 1);
                                    Trouble_DataGridView_SettingList.CurrentCell = Trouble_DataGridView_SettingList.Rows[Math.Min(selectedRowIndex, Trouble_DataGridView_SettingList.Rows.Count - 1)].Cells[0];
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                CustomMessage.Show(ex.ToString(), "エラー", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            }
        }

        /// <summary>
        /// DataGridViewの設定
        /// </summary>
        private void SetupDataGridView()
        {
            // 複数選択不可
            Trouble_DataGridView_SettingList.MultiSelect = false;
        }
    }
}
