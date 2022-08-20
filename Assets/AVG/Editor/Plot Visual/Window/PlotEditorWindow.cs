﻿using System.Linq;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEngine;
using UnityEngine.UIElements;

namespace AVG.Editor.Plot_Visual
{
    public class PlotEditorWindow : EditorWindow
    {
        private PlotGraphView m_GraphView;
        private PlotSo m_PlotSo;
        private string m_Title = "Plot Editor";

        public static void Edit(PlotSo targetPlot)
        {
            var window = GetWindow<PlotEditorWindow>("Plot Editor");
            window.m_PlotSo = targetPlot;
            window.GraphViewInitialize();
        }

        private void GraphViewInitialize()
        {
            m_GraphView ??= new PlotGraphView();
            m_GraphView.RegisterCallback<KeyDownEvent>(SpaceKeyMenu);
            m_GraphView.StretchToParentSize();
            rootVisualElement.Add(m_GraphView);
            m_GraphView.graphViewChanged += (_ =>
            {
                m_Title = "Plot Editor(Unsaved)";
                return default;
            });

            if (m_PlotSo.nodes == null)
            {
                m_PlotSo.ResetPlot();
                return;
            }

            #region Redraw the plot tree

            foreach (var data in m_PlotSo.nodes.ToList().Where(data => data != null))
            {
                m_GraphView.RedrawNode(data);
            }

            var listDictionary = m_PlotSo.links.ToDictionary(link => link.guid);
            var nodeList = m_GraphView.nodes.ToList().Cast<SectionNode>().ToList();
            var nodeDictionary = nodeList.ToDictionary(node => node.SectionData.Guid);

            foreach (var temp in from node in m_GraphView.nodes.ToList().Cast<SectionNode>().ToList()
                     where listDictionary.ContainsKey(node.SectionData.Guid)
                     let link = listDictionary[node.SectionData.Guid]
                     let targetNode = nodeDictionary[link.nextGuid]
                     select new Edge
                     {
                         output = node.outputContainer[0].Q<Port>(),
                         input = targetNode.inputContainer[0].Q<Port>(),
                     })
            {
                temp.input.Connect(temp);
                temp.output.Connect(temp);
                m_GraphView.Add(temp);
            }

            #endregion
        }

        private void SpaceKeyMenu(KeyDownEvent keyDownEvent)
        {
            if (keyDownEvent.keyCode != KeyCode.Space) return;
            var currentMousePosition = Event.current.mousePosition;
            var menu = new GenericMenu();
            menu.AddItem(new GUIContent("Add Node"), false,
                () => { m_GraphView.AddNode(currentMousePosition); });
            menu.AddItem(new GUIContent("Save"), false,
                DataSave);
            menu.ShowAsContext();
        }

        private void DataSave()
        {
            EditorUtility.SetDirty(m_PlotSo);
            m_PlotSo.ResetPlot();

            foreach (var sectionNode in m_GraphView.nodes.ToList().Cast<SectionNode>())
            {
                sectionNode.SectionData.nodePos = sectionNode.GetPosition();
                m_PlotSo.nodes.Add(sectionNode.SectionData);
            }

            var edgeList = m_GraphView.edges.ToList();
            for (var i = 0; i < edgeList.Count; i++)
            {
                var output = edgeList[i].output.node as SectionNode;
                var input = edgeList[i].input.node as SectionNode;


                m_PlotSo.links.Add(new NodeLink()
                {
                    guid = output?.SectionData.Guid,
                    nextGuid = input?.SectionData.Guid,
                    portId = i
                });
            }

            m_Title = "Plot Editor";
        }

        private void OnInspectorUpdate()
        {
            if (titleContent.text != m_Title) titleContent.text = m_Title;
        }
    }
}