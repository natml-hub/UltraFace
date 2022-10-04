/* 
*   UltraFace
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Vision {

    using System;
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;
    using NatML.Features;
    using NatML.Internal;
    using NatML.Types;

    /// <summary>
    /// Ultra-light Fast Generic Face Detector.
    /// This predictor accepts an image feature and produces a list of face rectangles.
    /// Face rectangles are always specified in normalized coordinates.
    /// </summary>
    public sealed class UltraFacePredictor : IMLPredictor<Rect[]> {

        #region --Client API--
        /// <summary>
        /// Create the UltraFace predictor.
        /// </summary>
        /// <param name="model">UltraFace ML model.</param>
        /// <param name="minScore">Minimum candidate score.</param>
        /// <param name="maxIoU">Maximum intersection-over-union score for overlap removal.</param>
        public UltraFacePredictor (MLModel model, float minScore = 0.5f, float maxIoU = 0.5f) {
            this.model = model as MLEdgeModel;
            this.minScore = minScore;
            this.maxIoU = maxIoU;
        }

        /// <summary>
        /// Detect faces in an image.
        /// </summary>
        /// <param name="inputs">Input image.</param>
        /// <returns>Detected faces.</returns>
        public Rect[] Predict (params MLFeature[] inputs) {
            // Check
            if (inputs.Length != 1)
                throw new ArgumentException(@"UltraFace predictor expects a single feature", nameof(inputs));
            // Check type
            var input = inputs[0];
            var imageType = MLImageType.FromType(input.type);
            var imageFeature = input as MLImageFeature;
            if (!imageType)
                throw new ArgumentException(@"UltraFace predictor expects an an array or image feature", nameof(inputs));        
            // Predict
            var inputType = model.inputs[0] as MLImageType;
            using var inputFeature = (input as IMLEdgeFeature).Create(inputType);
            using var outputFeatures = model.Predict(inputFeature);
            // Marshal
            var scores = new MLArrayFeature<float>(outputFeatures[0]);  // (1,P,2)
            var boxes = new MLArrayFeature<float>(outputFeatures[1]);   // (1,P,4)
            var candidateBoxes = new List<Rect>();
            var candidateScores = new List<float>();
            for (int i = 0, ilen = scores.shape[1]; i < ilen; ++i) {
                var score = scores[0,i,1];
                if (score < minScore)
                    continue;
                var rawBox = Rect.MinMaxRect(boxes[0,i,0], 1f - boxes[0,i,3], boxes[0,i,2], 1f - boxes[0,i,1]);
                var box = imageFeature?.TransformRect(rawBox, inputType) ?? rawBox;
                candidateBoxes.Add(box);
                candidateScores.Add(score);
            }
            var keepIdx = MLImageFeature.NonMaxSuppression(candidateBoxes, candidateScores, maxIoU);
            var result = keepIdx.Select(i => candidateBoxes[i]).ToArray();
            // Return
            return result;
        }
        #endregion


        #region --Operations--
        private readonly MLEdgeModel model;
        private readonly float minScore;
        private readonly float maxIoU;

        void IDisposable.Dispose () { } // Not used
        #endregion
    }
}