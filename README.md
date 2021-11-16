# NewUniverseProject
Testbed for ml-agents

Most of this is my own code and licensed under the MIT license. A few of the [example assets](Assets/ML-Agents/Examples/) I'm using
come from the [ml-agents repo](https://github.com/Unity-Technologies/ml-agents) and are licensed under Apache 2.0.

To see an example of subclassing `Agent` to control behavior, see the [Assets/Scripts](Assets/Scripts) folder, particularly [KinematicAgent](Assets/Scripts/KinematicAgent.cs). Training configs are in [Assets/Editor Default Resources/ML-Agents/config](Assets/Editor%20Default%20Resources/ML-Agents/config). My most recent training parameters depend on [RecurrentKinematicMoveToGoal.yaml](Assets/Editor%20Default%20Resources/ML-Agents/config/RecurrentKinematicMoveToGoal.yaml). A few of the more successful trained models can be found in [Assets/Models](Assets/Models). The only one of those that works with the current architecture is [Meeseeks.onnx](Assets/Models/Meeseeks.onnx). To use any of these models, edit the `Kinematic Environment` prefab, and attach it to the `Model` field of the `Behavior Parameters` component of the `Meeseeks` `GameObject`. I have made a few changes to the scoring system since training this model, which seems to have caused a regression in how well the model learns. Fixing this to make the model learn more effectively is left as an exercise for the reader.

If run without an `mlagents-learn` instance running, the project will automatically go into inference mode. To use this project for training, first follow the [instructions to install ml-agents](https://github.com/Unity-Technologies/ml-agents/blob/main/docs/Installation.md). Then you can train a new model by typing the following on the command line:
```
cd "Assets/Editor Default Resources/ML-Agents"
mlagents-learn config/RecurrentKinematicMoveToGoal.yaml --run-id MyRunID
```
After a brief initialization, `mlagents-learn` will say it is listening for a connection from a Unity instance. You can run the project at this point and it will connect to `mlagents-learn` and begin training. You can monitor the training progress by watching the agents in the scene view, seeing the scores reported by `mlagents-learn` on the console, or you can start a Tensorboard instance from this directory with `tensorboard --logdir results`, which will give you a graphical view of many useful training progress indicators. See the [ml-agents docs](https://github.com/Unity-Technologies/ml-agents/blob/main/docs/) for more details.

There is quite a lot to machine learning that I am still learning myself. This project provided a useful way to test out some fundamentals. I hope it can be useful to someone else. I am quite happy to answer any questions, so contact me if there's anything you'd like to know. I am TV4Fun#4708 on Discord and can be found other places on the web as well.
