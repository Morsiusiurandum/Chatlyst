﻿using System;
using UnityEditor.Experimental.GraphView;
using UnityEditor.UIElements;
using UnityEngine.UIElements;
using static UnityEngine.Resources;

namespace Chatlyst.Editor
{
    partial class ChatlystEditorWindow
    {
        //Basic element
        public static ChatlystEditorWindow EditorWindow;
        public static ChatlystGraphView    GraphView;

        //Toolbar element
        private ToolbarMenu   _toolbarMenu;
        private ToolbarToggle _inspectorToggle;
        private ToolbarToggle _autoSaveToggle;
        private ToolbarButton _saveButton;

        /// <summary>
        ///     Load the vision component
        /// </summary>
        /// <exception cref="Exception">Can not find EditorWindow.uxml</exception>
        private void ViewLoader()
        {
            // Build a tree of VisualElements from the asset.
            var visualTree = Load("UXML/NodeEditorWindow") as VisualTreeAsset;
            if (!visualTree) throw new Exception("Can not find EditorWindow.uxml");
            visualTree.CloneTree(rootVisualElement);
            TryGetVisualElement();
            EditorWindowInitialize();
        }

        /// <summary>
        ///     Get the individual visual elements
        /// </summary>
        private void TryGetVisualElement()
        {
            GraphView        = rootVisualElement.Q<ChatlystGraphView>("GraphView");
            _toolbarMenu     = rootVisualElement.Q<ToolbarMenu>("Menu");
            _inspectorToggle = rootVisualElement.Q<ToolbarToggle>("Inspector");
            _autoSaveToggle  = rootVisualElement.Q<ToolbarToggle>("AutoSave");
            _saveButton      = rootVisualElement.Q<ToolbarButton>("Save");
            //TODO:Check for null
        }

        /// <summary>
        ///     Initialize the open window
        /// </summary>
        private void EditorWindowInitialize()
        {
            saveChangesMessage = "Unsaved changes!\nDo you want to save?";
            titleContent.text  = _assetName;
            GraphView.Initialize(_jsonData);
            VisualElementMethodRegistration();

            //Reset the save flag
            hasUnsavedChanges = false;

            //TODO:Register for update and destroy events
        }

        /// <summary>
        ///     Register the callbacks for visual elements
        /// </summary>
        private void VisualElementMethodRegistration()
        {
            GraphView.graphViewChanged += OnGraphViewChanged();
            _saveButton.clicked        += OnSaveButtonClicked;
        }

        /// <summary>
        ///     The method triggered when the graph changes
        /// </summary>
        /// <returns>The delegate</returns>
        private GraphView.GraphViewChanged OnGraphViewChanged()
        {
            hasUnsavedChanges = true;
            return default;
        }

        /// <summary>
        ///     The method triggered when the save button was clicked
        /// </summary>
        private void OnSaveButtonClicked()
        {
            SaveChanges();
        }
    }
}
