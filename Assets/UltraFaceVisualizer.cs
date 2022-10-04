/* 
*   UltraFace
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Visualizers {

    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.UI;

    /// <summary>
    /// </summary>
    [RequireComponent(typeof(RawImage), typeof(AspectRatioFitter))]
    public sealed class UltraFaceVisualizer : MonoBehaviour {

        #region --Inspector--
        public Image faceRect;
        #endregion


        #region --Client API--
        /// <summary>
        /// Detection source image.
        /// </summary>
        public Texture2D image {
            get => rawImage.texture as Texture2D;
            set {
                rawImage.texture = value;
                aspectFitter.aspectRatio = (float)value.width / value.height;
            }
        }

        /// <summary>
        /// Render a set of detected faces.
        /// </summary>
        /// <param name="image">Image which detections are made on.</param>
        /// <param name="detections">Detections to render.</param>
        public void Render (params Rect[] detections) {
            // Delete current
            foreach (var rect in currentRects)
                GameObject.Destroy(rect.gameObject);
            currentRects.Clear();
            // Render rects
            var imageRect = new Rect(0, 0, image.width, image.height);
            foreach (var detection in detections) {
                var prefab = Instantiate(faceRect, transform);
                prefab.gameObject.SetActive(true);
                Render(prefab, detection);
                currentRects.Add(prefab);
            }
        }
        #endregion


        #region --Operations--
        private RawImage rawImage;
        private AspectRatioFitter aspectFitter;
        private readonly List<Image> currentRects = new List<Image>();

        void Awake () {
            rawImage = GetComponent<RawImage>();
            aspectFitter = GetComponent<AspectRatioFitter>();
        }

        void Render (Image prefab, Rect faceRect) {
            var rectTransform = prefab.transform as RectTransform;
            var imageTransform = rawImage.transform as RectTransform;
            rectTransform.anchorMin = 0.5f * Vector2.one;
            rectTransform.anchorMax = 0.5f * Vector2.one;
            rectTransform.pivot = Vector2.zero;
            rectTransform.sizeDelta = Vector2.Scale(imageTransform.rect.size, faceRect.size);
            rectTransform.anchoredPosition = Rect.NormalizedToPoint(imageTransform.rect, faceRect.position);
        }
        #endregion
    }
}