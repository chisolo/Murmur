UNITY_PATH="/c/Program Files/Unity/Hub/Editor/2021.3.10f1"

ROOT_ROOT=$(pwd)
PROJECT_PATH="${ROOT_ROOT}/Ose_Proj"
LOG_PATH="${ROOT_ROOT}/build.log"


"${UNITY_PATH}/Editor/Unity.exe" \
-batchmode \
-nographics \
-quit \
-projectPath "${PROJECT_PATH}" \
-logFile "${LOG_PATH}" \
-executeMethod App.BuildTool.BuildAAB