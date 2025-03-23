using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace HawkEye
{
    public partial class TreeView_test : Form
    {
        //private TreeNode testUsernameNode;
        private ContextMenuStrip treeViewContextMenu;
        private TreeNode previousTargetNode = null;
        private TreeNode previousDraggedNode = null;

        // コンストラクタ
        public TreeView_test()
        {
            InitializeComponent();
            //InitializeTreeView();
            InitializeTreeViewDragAndDrop();
            InitializeContextMenu();
            load_setting();
            this.FormClosing += new FormClosingEventHandler(TreeView_test_FormClosing);
            //treeView1.NodeMouseDoubleClick += new TreeNodeMouseClickEventHandler(treeView1_NodeMouseDoubleClick);
            //treeView1.AfterLabelEdit += new NodeLabelEditEventHandler(treeView1_AfterLabelEdit);
        }

        // コンテキストメニューの初期化
        private void InitializeContextMenu()
        {
            treeViewContextMenu = new ContextMenuStrip();
            //ToolStripMenuItem addItemMenuItem = new ToolStripMenuItem("項目の追加");
            ToolStripMenuItem addFolderMenuItem = new ToolStripMenuItem("フォルダの追加");
            ToolStripMenuItem renameMenuItem = new ToolStripMenuItem("名前の変更");
            ToolStripMenuItem changeCommandMenuItem = new ToolStripMenuItem("コマンドの変更");
            ToolStripMenuItem executeCommandMenuItem = new ToolStripMenuItem("コマンド実行");

            //addItemMenuItem.Click += new EventHandler(AddItemMenuItem_Click);
            addFolderMenuItem.Click += new EventHandler(AddFolderMenuItem_Click);
            renameMenuItem.Click += new EventHandler(RenameMenuItem_Click);
            changeCommandMenuItem.Click += new EventHandler(ChangeCommandMenuItem_Click);
            executeCommandMenuItem.Click += new EventHandler(ExecuteCommandMenuItem_Click);

            treeViewContextMenu.Items.AddRange(new ToolStripItem[] { 
                //addItemMenuItem,
                addFolderMenuItem, 
                renameMenuItem,
                changeCommandMenuItem,
                executeCommandMenuItem,
            });
            treeView1.ContextMenuStrip = treeViewContextMenu;
        }

        // コマンドの変更
        private void ChangeCommandMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode != null)
            {
                using (InputBox inputBox = new InputBox("コマンドを入力してください:", "コマンドの変更", selectedNode.Tag?.ToString()))
                {
                    if (inputBox.ShowDialog() == DialogResult.OK)
                    {
                        string input = inputBox.InputText;
                        if (!string.IsNullOrEmpty(input))
                        {
                            selectedNode.Tag = input;
                            //MessageBox.Show("コマンドが変更されました: " + input);
                        }
                    }
                }
            }
        }


        // コマンド実行
        private void ExecuteCommandMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode != null && selectedNode.Tag != null)
            {
                string command = selectedNode.Tag.ToString();
                try
                {
                    // コマンドを実行する
                    //System.Diagnostics.Process.Start(command);

                    // コマンドと引数を分ける
                    string[] commandParts = Regex.Matches(command, @"[\""].+?[\""]|[^ ]+")
                                                 .Cast<Match>()
                                                 .Select(m => m.Value)
                                                 .ToArray();
                    string fileName = commandParts[0].Trim('"');
                    string arguments = commandParts.Length > 1 ? string.Join(" ", commandParts.Skip(1)) : string.Empty;

                    // ProcessStartInfoを使用してプロセスを開始
                    ProcessStartInfo startInfo = new ProcessStartInfo
                    {
                        FileName = fileName,
                        Arguments = arguments
                    };
                    System.Diagnostics.Process.Start(startInfo);


                }
                catch (Exception ex)
                {
                    MessageBox.Show("コマンドの実行に失敗しました: " + ex.Message);
                }
            }
            else
            {
                MessageBox.Show("コマンドが設定されていません。");
            }
        }


        // 項目の追加
        private void AddItemMenuItem_Click(object sender, EventArgs e)
        {
            // 項目の追加処理をここに記述
        }

        private void AddFolderMenuItem_Click(object sender, EventArgs e)
        {

            TreeNode selectedNode = treeView1.SelectedNode;
            TreeNode newNode = new TreeNode("New");
            if (selectedNode != null)
            {
                selectedNode.Nodes.Add(newNode);
                selectedNode.Expand();
            }
            else
            {
                treeView1.Nodes.Add(newNode);
            }

        }

        // 名前の変更
        private void RenameMenuItem_Click(object sender, EventArgs e)
        {
            TreeNode selectedNode = treeView1.SelectedNode;
            if (selectedNode != null)
            {
                selectedNode.BeginEdit();
            }
        }

        // 設定情報の取得
        private void load_setting()
        {
            //string test_username = Properties.Settings.Default.UserName;
            //testUsernameNode = new TreeNode(test_username);
            //treeView1.Nodes.Add(testUsernameNode);

            // [UserName]の設定を取得
            //string test_username = Properties.Settings.Default.UserName;
            //testUsernameNode = new TreeNode(test_username);
            //treeView1.Nodes.Add(testUsernameNode);

            string serializedTreeView = Properties.Settings.Default.TreeViewData;
            if (!string.IsNullOrEmpty(serializedTreeView))
            {
                DeserializeTreeView(treeView1, serializedTreeView);
            }
            // 全てのノードを展開
            treeView1.ExpandAll();


        }

        // ツリービューの内容をシリアライズ
        private string SerializeTreeView(TreeView treeView)
        {
            StringBuilder sb = new StringBuilder();
            foreach (TreeNode node in treeView.Nodes)
            {
                SerializeNode(node, sb, 0);
            }
            return sb.ToString();
        }

        // ノードをシリアライズ
        private void SerializeNode(TreeNode node, StringBuilder sb, int level)
        {
            string tag = node.Tag != null ? node.Tag.ToString() : string.Empty;
            //sb.AppendLine(new string(' ', level * 2) + node.Text);
            sb.AppendLine(new string(' ', level * 2) + node.Text + "|" + tag);
            foreach (TreeNode childNode in node.Nodes)
            {
                SerializeNode(childNode, sb, level + 1);
            }
        }


        //// フォーム終了時に設定を保存
        //private void TreeView_test_FormClosing(object sender, FormClosingEventArgs e)
        //{
        //    string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
        //    Properties.Settings.Default.UserName = "new_username" + currentTime;
        //    Properties.Settings.Default.Save();
        //}

        /*
        private void InitializeTreeView()
        {
            // ノードを追加
            TreeNode rootNode1 = new TreeNode("Root Node1");
            rootNode1.Nodes.Add("Child Node 1-1");
            rootNode1.Nodes.Add("Child Node 1-2");
            rootNode1.Nodes.Add("Child Node 1-3");

            TreeNode rootNode2 = new TreeNode("Root Node2");
            rootNode2.Nodes.Add("Child Node 2-1");
            rootNode2.Nodes.Add("Child Node 2-2");
            rootNode2.Nodes.Add("Child Node 2-3");
            rootNode1.Nodes.Add(rootNode2);

            treeView1.Nodes.Add(rootNode1);

            // 全てのノードを展開
            //treeView1.ExpandAll();
        }
        */

        private void InitializeTreeViewDragAndDrop()
        {
            treeView1.AllowDrop = true;
            treeView1.ItemDrag += new ItemDragEventHandler(treeView1_ItemDrag);
            treeView1.DragEnter += new DragEventHandler(treeView1_DragEnter);
            treeView1.DragOver += new DragEventHandler(treeView1_DragOver);
            treeView1.DragDrop += new DragEventHandler(treeView1_DragDrop);
            treeView1.DragLeave += new EventHandler(treeView1_DragLeave); // ドラッグがキャンセルされた場合の処理を追加
        }

        // ドラッグ開始
        private void treeView1_ItemDrag(object sender, ItemDragEventArgs e)
        {
            previousDraggedNode = e.Item as TreeNode;
            if (previousDraggedNode != null && previousDraggedNode.Text != "Root Node1")
            {
                //previousDraggedNode.BackColor = Color.LightGreen; // ドラッグしているノードの色を変更
                //treeView1.SelectedNode = previousDraggedNode; // 移動中のノードにフォーカスを当てる
                DoDragDrop(e.Item, DragDropEffects.Move);
            }
        }

        // ドラッグ中
        private void treeView1_DragEnter(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                e.Effect = DragDropEffects.Move;
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        // ドラッグ中
        private void treeView1_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                e.Effect = DragDropEffects.Move;

                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                TreeNode targetNode = ((TreeView)sender).GetNodeAt(pt);

                if (previousTargetNode != null && previousTargetNode != targetNode)
                {
                    previousTargetNode.BackColor = treeView1.BackColor; // 前回のドロップ対象ノードの色を元に戻す
                }

                if (targetNode != null && targetNode != previousTargetNode)
                {
                    targetNode.BackColor = Color.LightBlue; // ドロップ対象ノードの色を変更
                    previousTargetNode = targetNode;
                }
            }
            else
            {
                e.Effect = DragDropEffects.None;
            }
        }

        // ドロップ
        private void treeView1_DragDrop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(typeof(TreeNode)))
            {
                TreeNode newNode = (TreeNode)e.Data.GetData(typeof(TreeNode));
                Point pt = ((TreeView)sender).PointToClient(new Point(e.X, e.Y));
                TreeNode targetNode = ((TreeView)sender).GetNodeAt(pt);

                if (targetNode != null && newNode != targetNode && !ContainsNode(newNode, targetNode))
                {
                    
                    Console.WriteLine("ドロップ先: " + targetNode.Text);

                    // ドラッグしているノードの色を元に戻す
                    if (previousDraggedNode != null)
                    {
                        Console.WriteLine("previousDraggedNode: " + previousDraggedNode.Text);
                    //    previousDraggedNode.BackColor = treeView1.BackColor;
                    //    previousDraggedNode = null;
                    }

                    newNode.Remove();
                    targetNode.Nodes.Add(newNode);
                    targetNode.Expand();
                    newNode.EnsureVisible();
                    treeView1.SelectedNode = newNode; // 移動したノードにフォーカスを当てる

                    // ドロップ後に色を元に戻す
                    if (previousTargetNode != null)
                    {
                        Console.WriteLine("previousTargetNode: " + previousTargetNode.Text);
                        previousTargetNode.BackColor = treeView1.BackColor;
                        previousTargetNode = null;
                    }
                }
            }
        }

        // ノードが含まれているか
        private bool ContainsNode(TreeNode node1, TreeNode node2)
        {
            if (node2.Parent == null) return false;
            if (node2.Parent == node1) return true;
            return ContainsNode(node1, node2.Parent);
        }

        // ノードが選択された時の処理
        private void treeView1_AfterSelect(object sender, TreeViewEventArgs e)
        {
            Console.WriteLine("Selected Node: " + e.Node.Text);
        }

        //// ノードのラベルを編集可能にする
        //private void treeView1_NodeMouseDoubleClick(object sender, TreeNodeMouseClickEventArgs e)
        //{
        //    e.Node.BeginEdit();
        //}

        /*
        // ノードのラベル編集後の処理
        private void treeView1_AfterLabelEdit(object sender, NodeLabelEditEventArgs e)
        {
            if (e.Node == testUsernameNode && e.Label != null)
            {
                //string currentTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"); // デバッグ用に現在時刻を追加
                //Properties.Settings.Default.UserName = e.Label + currentTime;
                Properties.Settings.Default.UserName = e.Label;
                Properties.Settings.Default.Save();
                Console.WriteLine("設定に保存" + e.Label);
            }
        }
        */

        // ドラッグがキャンセルされた場合
        private void treeView1_DragLeave(object sender, EventArgs e)
        {
            if (previousTargetNode != null)
            {
                previousTargetNode.BackColor = treeView1.BackColor;
                previousTargetNode = null;
            }

            //if (previousDraggedNode != null)
            //{
            //    previousDraggedNode.BackColor = treeView1.BackColor;
            //    previousDraggedNode = null;
            //}
        }

        // フォーム終了時に設定を保存
        private void TreeView_test_FormClosing(object sender, FormClosingEventArgs e)
        {
            string serializedTreeView = SerializeTreeView(treeView1);
            Properties.Settings.Default.TreeViewData = serializedTreeView;
            Properties.Settings.Default.Save();
        }

        // フォームがロードされた時に設定を読み込む
        private void DeserializeTreeView(TreeView treeView, string data)
        {
            treeView.Nodes.Clear();
            string[] lines = data.Split(new[] { Environment.NewLine }, StringSplitOptions.None);
            Stack<TreeNode> stack = new Stack<TreeNode>();
            foreach (string line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                int level = line.TakeWhile(char.IsWhiteSpace).Count() / 2;
                string[] parts = line.Trim().Split('|');
                string text = parts[0];
                string tag = parts.Length > 1 ? parts[1] : string.Empty;
                TreeNode newNode = new TreeNode(text) { Tag = tag };
                if (stack.Count == 0)
                {
                    treeView.Nodes.Add(newNode);
                }
                else
                {
                    while (stack.Count > level)
                    {
                        stack.Pop();
                    }
                    if (stack.Count > 0)
                    {
                        stack.Peek().Nodes.Add(newNode);
                    }
                    else
                    {
                        treeView.Nodes.Add(newNode);
                    }
                }
                stack.Push(newNode);
            }
        }

    }
}

