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
using UnityEngine.UI;

namespace UI
{
    public class Arrow : MonoBehaviour
    {
        public float ActiveTime = 0.25f;

        public Sprite InactiveSprite;
        public Sprite ActiveSprite;

        public Image Image => image;
        private Image image;
        private float activeTime = 0f;

        public void Flash()
        {
            activeTime = ActiveTime;
        }

        private void Update()
        {
            if (activeTime > 0f)
                activeTime -= Time.deltaTime;
            image.sprite = (activeTime > 0f) ? ActiveSprite : InactiveSprite;
        }

        private void Awake()
        {
            image = GetComponent<Image>();
        }
    }
}