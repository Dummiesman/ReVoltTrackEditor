/*
    Re-Volt Track Editor - Unity Edition
    A version of the track editor re-built from the ground up in Unity
    Copyright (C) 2022 Dummiesman

    This program is free software: you can redistribute it and/or modify
    it under the terms of the GNU General Public License as published by
    the Free Software Foundation, either version 3 of the License, or
    (at your option) any later version.

    This program is distributed in the hope that it will be useful,
    but WITHOUT ANY WARRANTY; without even the implied warranty of
    MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
    GNU General Public License for more details.

    You should have received a copy of the GNU General Public License
    along with this program.  If not, see <http://www.gnu.org/licenses/>.
*/

using ReVolt.TrackUnit.Unity;
using System.Collections.Generic;
using UnityEngine;

public class ModuleSelectMode : EditorMode
{
    // Virtual properties
    public override string ModeName => Localization.Lookup(LocString.MODE_MODULES);

    //scene references
    public RectTransform Viewport;
    public Camera ModuleCamera;
    public TrackEditingMode EditingMode;

    public RectTransform ModuleListContainer;
    public GameObject ModuleListItemPrefab;

    //
    private Vector2Int cursorPosition = Vector2Int.zero;

    private readonly Vector3 ModulePreviewPosition = new Vector3(-999f, 0f, -999f); //far away from the main grid
    
    private int activeModuleIndex => Modules.Lookup.Groups[cursorPosition.y].ModuleIDs[cursorPosition.x];
    private List<GameObject[]> moduleGroups = new List<GameObject[]>();
    private UI.ModuleItem[] moduleUiGroups;

    private const int NUM_CHOOSABLE_MODULES = 15;

    // Others
    private void InitList()
    {
        moduleUiGroups = new UI.ModuleItem[Modules.Lookup.Groups.Length];
        for (int i=0; i < Modules.Lookup.Groups.Length; i++)
        {
            var group = Modules.Lookup.Groups[i];

            var pref = Instantiate(ModuleListItemPrefab, ModuleListContainer.transform, false);
            var uiComponent = pref.GetComponent<UI.ModuleItem>();
            moduleUiGroups[i] = uiComponent;

            uiComponent.Init(group);
        }
    }

    private void UpdateCamera()
    {
        // rotate the camera around the module
        ModuleCamera.transform.RotateAround(ModulePreviewPosition, Vector3.up, Time.deltaTime * -45f);

        // adjust cam viewport
        ModuleCamera.pixelRect = Viewport.GetScreenCoordinates();
    }

    private void OnActiveModuleChanged()
    {
        for (int y = 0; y < moduleGroups.Count; y++)
        {
            //ui state
            if(y == cursorPosition.y)
                moduleUiGroups[y].Select(cursorPosition.x);
            else
                moduleUiGroups[y].Deselect();

            //3d state
            for(int x = 0; x < 2; x++)
            {
                var mod = moduleGroups[y][x];
                mod.SetActive(y == cursorPosition.y && x == cursorPosition.x);
            }
        }
    }

    // Virtuals
    public override void Init()
    {
        base.Init();

        // init ui
        InitList();

        // clone modules for our use
        List<GameObject> allModules = new List<GameObject>();
        moduleGroups = new List<GameObject[]>();

        for(int y=0; y < Modules.Lookup.Groups.Length; y++)
        {
            GameObject[] array = new GameObject[2];
            for(int x=0; x < 2; x++)
            {
                var instantiatedModule = Instantiate(TrackEditor.ProcessedTrackUnit.UnityModules[Modules.Lookup.Groups[y].ModuleIDs[x]], this.transform);
                allModules.Add(instantiatedModule);
                array[x] = instantiatedModule;
            }
            moduleGroups.Add(array);
        }

        // initialize the modules
        foreach(var modObj in allModules)
        {
            var modInst = modObj.GetComponent<ModuleInstance>();
            var module = TrackEditor.TrackUnit.Modules[modInst.IndexInFile];
            var bounds = module.CalculateModuleGridSize();

            modInst.SetModuleScreenConfig();

            // scale and position
            float xDiv = Mathf.Max(1, bounds.size.x);
            float yDiv = Mathf.Max(1, bounds.size.y);
            float scaleDiv = Mathf.Max(xDiv, yDiv);

            float xOffset = Mathf.Max(0f, xDiv - 1f) * 0.25f; // offset 0.25f for every additional unit in the module
            float zOffset = Mathf.Max(0f, yDiv - 1f) * -0.25f; // offset 0.25f for every additional unit in the module

            // calculate bounds to offset height
            Bounds total3DBounds = new Bounds(Vector3.zero, Vector3.zero);
            foreach(var unit in modInst.Units)
            {
                if(unit.PegObject != null)
                {
                    foreach (var renderer in unit.PegObject.GetComponentsInChildren<Renderer>())
                        total3DBounds.Encapsulate(renderer.bounds);
                }
                if (unit.PanObject != null)
                {
                    foreach (var renderer in unit.PanObject.GetComponentsInChildren<Renderer>())
                        total3DBounds.Encapsulate(renderer.bounds);
                }
            }

            modObj.transform.position = ModulePreviewPosition - new Vector3(xOffset, total3DBounds.extents.y, zOffset);
            modObj.transform.localScale /= scaleDiv;
        }

        OnActiveModuleChanged();
    }

    public override void Update()
    {
        base.Update();

        UpdateCamera();
    }

    public override void UpdateInput()
    {
        // movement
        var movementDelta = Controls.DirectionalMovement.GetMovementDelta();
        if(movementDelta.sqrMagnitude > 0)
        {
            TrackEditor.PlaySound(TrackEditor.SndMenuMove);
            movementDelta.y *= -1; // down should increase the cursor position in this mode
            cursorPosition += movementDelta;
            cursorPosition.x = (cursorPosition.x < 0) ? 1 : cursorPosition.x % 2;
            cursorPosition.y = (cursorPosition.y < 0) ? NUM_CHOOSABLE_MODULES - 1 : cursorPosition.y % NUM_CHOOSABLE_MODULES;
            OnActiveModuleChanged();
        }

        // choosing
        if (Controls.UI.ConfirmPressed())
        {
            EditingMode.SetActiveModule(activeModuleIndex);
            TrackEditor.Instance.SetTrackEditingMode();
        }
        if (Controls.UI.BackPressed())
        {
            TrackEditor.Instance.SetTrackEditingMode();
        }

        base.UpdateInput();
    }

    public override void OnEnterMode()
    {
        base.OnEnterMode();

        // setup camera
        ModuleCamera.gameObject.SetActive(true);
        ModuleCamera.transform.position = ModulePreviewPosition - (Vector3.forward * 1.5f) + (Vector3.up * 1.5f);
        ModuleCamera.transform.LookAt(ModulePreviewPosition);
        UpdateCamera();

        TrackEditor.Instance.Camera.gameObject.SetActive(false);
    }

    public override void OnExitMode()
    {
        base.OnExitMode();

        // turn off our camera, and the main camera back on
        ModuleCamera.gameObject.SetActive(false);
        TrackEditor.Instance.Camera.gameObject.SetActive(true);
    }
}
