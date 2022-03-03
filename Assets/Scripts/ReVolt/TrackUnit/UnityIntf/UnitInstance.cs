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

using UnityEngine;

namespace ReVolt.TrackUnit.Unity
{
    public class UnitInstance : MonoBehaviour
    {
        public int IndexInFile;

        public GameObject PanObject => panObject;
        public GameObject PegObject => pegObject;
        public GameObject HullObject => hullObject;
        public GameObject RootObject => rootObject;

        [SerializeField]
        private GameObject panObject;

        [SerializeField]
        private GameObject pegObject;

        [SerializeField]
        private GameObject hullObject;

        [SerializeField]
        private GameObject rootObject;

        [SerializeField]
        private bool isEditorConfig = false;

        public void Init(GameObject unitObj)
        {
            this.panObject = unitObj.transform.Find("Pan").gameObject;
            this.pegObject = unitObj.transform.Find("Peg").gameObject;
            this.hullObject = unitObj.transform.Find("Hull").gameObject;
            this.rootObject = unitObj.transform.Find("Root").gameObject;
            SetRootHeight(0f);
        }
        
        public void SetRootHeight(float height)
        {
            if (rootObject != null)
            {
                rootObject.SetActive(isEditorConfig && height != 0f);      
                rootObject.transform.localScale = new Vector3(1f, height * RVConstants.SMALL_CUBE_SIZE, 1f);
            }
        }

        public void SetModuleScreenConfig()
        {
            if (panObject != null)
                panObject.SetActive(true);
            if (pegObject != null)
                pegObject.SetActive(true);
            if (hullObject != null)
                hullObject.SetActive(false);
            if (rootObject != null)
                rootObject.SetActive(false);
            isEditorConfig = false;
        }

        public void SetEditorConfig()
        {
            if (panObject != null)
                panObject.SetActive(true);
            if (pegObject != null)
                pegObject.SetActive(true);
            if (hullObject != null)
                hullObject.SetActive(false);
            if (rootObject != null)
                rootObject.SetActive(true);
            isEditorConfig = true;
        }
    }
}
