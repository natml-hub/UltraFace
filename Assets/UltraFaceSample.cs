/* 
*   UltraFace
*   Copyright (c) 2022 NatML Inc. All Rights Reserved.
*/

namespace NatML.Examples {

    using System.Threading.Tasks;
    using UnityEngine;
    using NatML.Devices;
    using NatML.Devices.Outputs;
    using NatML.Features;
    using NatML.Vision;
    using NatML.Visualizers;

    public sealed class UltraFaceSample : MonoBehaviour {

        [Header(@"UI")]
        public UltraFaceVisualizer visualizer;

        private CameraDevice cameraDevice;
        private TextureOutput cameraTextureOutput;

        private MLModelData modelData;
        private MLModel model;
        private UltraFacePredictor predictor;

        async void Start () {
            // Request camera permissions
            var permissionStatus = await MediaDeviceQuery.RequestPermissions<CameraDevice>();
            if (permissionStatus != PermissionStatus.Authorized) {
                Debug.LogError(@"User did not grant camera permissions");
                return;
            }
            // Get a camera device
            var query = new MediaDeviceQuery(MediaDeviceCriteria.CameraDevice);
            cameraDevice = query.current as CameraDevice;
            // Start the camera preview
            cameraTextureOutput = new TextureOutput();
            cameraDevice.StartRunning(cameraTextureOutput);
            // Display the camera preview
            var cameraTexture = await cameraTextureOutput;
            visualizer.image = cameraTexture;
            // Fetch the model from NatML
            modelData = await MLModelData.FromHub("@natsuite/ultraface");
            model = modelData.Deserialize();
            predictor = new UltraFacePredictor(model);
        }

        void Update () {
            // Check that predictor has created
            if (predictor == null)
                return;
            // Create input feature
            var imageFeature = new MLImageFeature(cameraTextureOutput.texture);
            (imageFeature.mean, imageFeature.std) = modelData.normalization;
            imageFeature.aspectMode = modelData.aspectMode;
            // Predict
            var faces = predictor.Predict(imageFeature);
            // Visualize
            visualizer.Render(faces);
        }

        void OnDisable () {
            // Dispose the model
            model?.Dispose();
        }
    }
}