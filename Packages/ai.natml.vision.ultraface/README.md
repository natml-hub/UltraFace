# UltraFace
[Ultra-light Fast Generic Face Detector](https://github.com/Linzaer/Ultra-Light-Fast-Generic-Face-Detector-1MB) for high performance face detection.

## Installing UltraFace
Add the following items to your Unity project's `Packages/manifest.json`:
```json
{
  "scopedRegistries": [
    {
      "name": "NatML",
      "url": "https://registry.npmjs.com",
      "scopes": ["ai.natml"]
    }
  ],
  "dependencies": {
    "ai.natml.vision.ultraface": "1.0.0"
  }
}
```

## Detecting Faces in an Image
First, create the UltraFace predictor:
```csharp
// Fetch the model data from NatML Hub
var modelData = await MLModelData.FromHub("@natsuite/ultraface");
// Deserialize the model
var model = modelData.Deserialize();
// Create the UltraFace predictor
var predictor = new UltraFacePredictor(model);
```

Then detect faces in the image:
```csharp
// Create image feature
Texture2D image = ...;
var imageFeature = new MLImageFeature(image); // This also accepts a `Color32[]` or `byte[]`
(imageFeature.mean, imageFeature.std) = modelData.normalization;
imageFeature.aspectMode = modelData.aspectMode;
// Detect faces
Rect[] faces = predictor.Predict(imageFeature);
```

___

## Requirements
- Unity 2021.2+

## Quick Tips
- Join the [NatML community on Discord](https://hub.natml.ai/community).
- Discover more ML models on [NatML Hub](https://hub.natml.ai).
- See the [NatML documentation](https://docs.natml.ai/unity).
- Discuss [NatML on Unity Forums](https://forum.unity.com/threads/open-beta-natml-machine-learning-runtime.1109339/).
- Contact us at [hi@natml.ai](mailto:hi@natml.ai).

Thank you very much!