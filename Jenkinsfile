#!groovy

def COLOR_MAP = [
    'SUCCESS': 'good', 
    'FAILURE': 'danger',
]

pipeline {
    
    environment {
        PROJECT_NAME = "MicroparticlesVisualization-ILIAD"
        
        // Unity tool installation
        UNITY_EXECUTABLE = "C:\\Program Files\\Unity\\Hub\\Editor\\2022.3.7f1\\Editor\\Unity.exe"
        
        // Latest curl version installation
        CURL_EXECUTABLE = "C:\\Program Files\\Git\\mingw64\\bin\\curl.exe"

        // Unity Build params & paths
        WINDOWS_BUILD_NAME = "Windows-${currentBuild.number}"
        WINDOWS_DEV_BUILD_NAME = "Windows-Dev-${currentBuild.number}"
        MACOS_BUILD_NAME = "MacOS-${currentBuild.number}"
        MACOS_DEV_BUILD_NAME = "MacOS-Dev-${currentBuild.number}"
        ANDROID_BUILD_NAME = "Android-${currentBuild.number}"
        ANDROID_DEV_BUILD_NAME = "Android-Dev-${currentBuild.number}"
        IOS_BUILD_NAME = "IOS-${currentBuild.number}"
        IOS_DEV_BUILD_NAME = "IOS-Dev-${currentBuild.number}"
        WEBGL_BUILD_NAME = "WebGL-${currentBuild.number}"
        WEBGL_DEV_BUILD_NAME = "WebGL-Dev-${currentBuild.number}"
        
        String output = "Output"
        String outputMacOSDevFolder = "CurrentMacDevBuild"
        String outputWindowsDevFolder = "CurrentWinDevBuild"
        String outputAndroidDevFolder = "CurrentAndroidDevBuild"
        String outputWebGLDevFolder = "CurrentWebGLDevBuild"
        String outputIosDevFolder = "CurrentIosDevBuild"
        String outputMacOSFolder = "CurrentMacBuild"
        String outputWindowsFolder = "CurrentWinBuild"
        String outputAndroidFolder = "CurrentAndroidBuild"
        String outputWebGLFolder = "CurrentWebGLBuild"
        String outputIosFolder = "CurrentIosBuild"
        
        NEXUS_CREDENTIALS = credentials('NEXUS_CREDENTIALS')
    }
    
    options {
        timestamps()
    }
    
    agent {
            node {
                    label 'windows'
        }
    }
    
    stages {
            stage('Clone Script') {
                    steps {
                        echo "Cloning the branch commit"
                        checkout scm
                        echo "Fetching tags"
                        bat '''git fetch --all --tags'''
                }
        }
        
        stage('Build Pull Request') {
        
            when { 
                    expression { env.BRANCH_NAME.startsWith('PR') }
                }
            steps {
                script {
                    
                    echo "Launching Windows Development Build..."
                    bat '''"%UNITY_EXECUTABLE%" -projectPath "%CD%" -quit -batchmode -nographics -customBuildPath "%CD%\\%output%\\%outputWindowsDevFolder%\\%PROJECT_NAME%.exe" -customBuildName "%PROJECT_NAME%" -executeMethod BuildUtility.WindowsDevBuilder'''
                    
                    echo "Launching MacOS Development Build..."
                    bat '''"%UNITY_EXECUTABLE%" -projectPath "%CD%" -quit -batchmode -nographics -customBuildPath "%CD%\\%output%\\%outputMacOSDevFolder%\\%PROJECT_NAME%.exe" -customBuildName "%PROJECT_NAME%" -executeMethod BuildUtility.MacOSDevBuilder'''
                    
                    echo "Launching WebGL Development Build..."
                    bat '''"%UNITY_EXECUTABLE%" -projectPath "%CD%" -quit -batchmode -nographics -customBuildPath "%CD%\\%output%\\%outputWebGLDevFolder%\\%PROJECT_NAME%.exe" -customBuildName "%PROJECT_NAME%" -executeMethod BuildUtility.WebGLDevBuilder'''
                    
                }
            }
        }
        
        stage('Build Dev Branch') {
        
            when { 
                    expression { BRANCH_NAME ==~ /(dev)/ }
                }
            steps {
                script {
                    
                    echo "Launching Windows Development Build..."
                    bat '''"%UNITY_EXECUTABLE%" -projectPath "%CD%" -quit -batchmode -nographics -customBuildPath "%CD%\\%output%\\%outputWindowsDevFolder%\\%PROJECT_NAME%.exe" -customBuildName "%PROJECT_NAME%" -executeMethod BuildUtility.WindowsDevBuilder'''

                    echo "Zipping build..."
                    bat '''7z a -tzip -r "%output%\\%WINDOWS_DEV_BUILD_NAME%" "%CD%\\%output%\\%outputWindowsDevFolder%\\*"'''

                    echo "Uploading dev build artifact to Nexus..."
                    bat '''"%CURL_EXECUTABLE%" -X POST "http://localhost:8081/service/rest/v1/components?repository=%PROJECT_NAME%-Dev" -H "accept: application/json" -H "Authorization: Basic %NEXUS_CREDENTIALS%" -F "raw.directory=Windows" -F "raw.asset1=@%output%\\%WINDOWS_DEV_BUILD_NAME%.zip;type=application/x-zip-compressed" -F "raw.asset1.filename=%WINDOWS_DEV_BUILD_NAME%.zip"'''
                    
                    echo "Launching MacOS Development Build..."
                    bat '''"%UNITY_EXECUTABLE%" -projectPath "%CD%" -quit -batchmode -nographics -customBuildPath "%CD%\\%output%\\%outputMacOSDevFolder%\\%PROJECT_NAME%.exe" -customBuildName "%PROJECT_NAME%" -executeMethod BuildUtility.MacOSDevBuilder'''

                    echo "Zipping build..."
                    bat '''7z a -tzip -r "%output%\\%MACOS_DEV_BUILD_NAME%" "%CD%\\%output%\\%outputMacOSDevFolder%\\*"'''

                    echo "Uploading dev build artifact to Nexus..."
                    bat '''"%CURL_EXECUTABLE%" -X POST "http://localhost:8081/service/rest/v1/components?repository=%PROJECT_NAME%-Dev" -H "accept: application/json" -H "Authorization: Basic %NEXUS_CREDENTIALS%" -F "raw.directory=MacOS" -F "raw.asset1=@%output%\\%MACOS_DEV_BUILD_NAME%.zip;type=application/x-zip-compressed" -F "raw.asset1.filename=%MACOS_DEV_BUILD_NAME%.zip"'''
                                        
                    echo "Launching WebGL Development Build..."
                    bat '''"%UNITY_EXECUTABLE%" -projectPath "%CD%" -quit -batchmode -nographics -customBuildPath "%CD%\\%output%\\%outputWebGLDevFolder%\\%PROJECT_NAME%.exe" -customBuildName "%PROJECT_NAME%" -executeMethod BuildUtility.WebGLDevBuilder'''

                    echo "Zipping build..."
                    bat '''7z a -tzip -r "%output%\\%WEBGL_DEV_BUILD_NAME%" "%CD%\\%output%\\%outputWebGLDevFolder%\\*"'''

                    echo "Uploading dev build artifact to Nexus..."
                    bat '''"%CURL_EXECUTABLE%" -X POST "http://localhost:8081/service/rest/v1/components?repository=%PROJECT_NAME%-Dev" -H "accept: application/json" -H "Authorization: Basic %NEXUS_CREDENTIALS%" -F "raw.directory=WebGL" -F "raw.asset1=@%output%\\%WEBGL_DEV_BUILD_NAME%.zip;type=application/x-zip-compressed" -F "raw.asset1.filename=%WEBGL_DEV_BUILD_NAME%.zip"'''
                }
            }
        }
        
        stage('Build Main Branch') {
        
            when { 
                    expression { BRANCH_NAME ==~ /(main)/ }
                }
            steps {
                script {
                    
                    echo "Launching Windows Release Build..."
                    bat '''"%UNITY_EXECUTABLE%" -projectPath "%CD%" -quit -batchmode -nographics -customBuildPath "%CD%\\%output%\\%outputWindowsFolder%\\%PROJECT_NAME%.exe" -customBuildName "%PROJECT_NAME%" -executeMethod BuildUtility.WindowsBuilder'''
                    
                    echo "Zipping build..."
                    bat '''7z a -tzip -r "%output%\\%WINDOWS_BUILD_NAME%" "%CD%\\%output%\\%outputWindowsFolder%\\*"'''
                        
                    echo "Uploading release build artifact to Nexus..."                        
                    bat '''"%CURL_EXECUTABLE%" -X POST "http://localhost:8081/service/rest/v1/components?repository=%PROJECT_NAME%-Main" -H "accept: application/json" -H "Authorization: Basic %NEXUS_CREDENTIALS%" -F "raw.directory=Windows" -F "raw.asset1=@%output%\\%WINDOWS_BUILD_NAME%.zip;type=application/x-zip-compressed" -F "raw.asset1.filename=%WINDOWS_BUILD_NAME%.zip"'''
                    
                    echo "Launching MacOS Release Build..."
                    bat '''"%UNITY_EXECUTABLE%" -projectPath "%CD%" -quit -batchmode -nographics -customBuildPath "%CD%\\%output%\\%outputMacOSFolder%\\%PROJECT_NAME%.exe" -customBuildName "%PROJECT_NAME%" -executeMethod BuildUtility.MacOSBuilder'''
                    
                    echo "Zipping build..."
                    bat '''7z a -tzip -r "%output%\\%MACOS_BUILD_NAME%" "%CD%\\%output%\\%outputMacOSFolder%\\*"'''
                    
                    echo "Uploading release build artifact to Nexus..."                        
                    bat '''"%CURL_EXECUTABLE%" -X POST "http://localhost:8081/service/rest/v1/components?repository=%PROJECT_NAME%-Main" -H "accept: application/json" -H "Authorization: Basic %NEXUS_CREDENTIALS%" -F "raw.directory=MacOS" -F "raw.asset1=@%output%\\%MACOS_BUILD_NAME%.zip;type=application/x-zip-compressed" -F "raw.asset1.filename=%MACOS_BUILD_NAME%.zip"'''
                                        
                    echo "Launching WebGL Release Build..."
                    bat '''"%UNITY_EXECUTABLE%" -projectPath "%CD%" -quit -batchmode -nographics -customBuildPath "%CD%\\%output%\\%outputWebGLFolder%\\%PROJECT_NAME%.exe" -customBuildName "%PROJECT_NAME%" -executeMethod BuildUtility.WebGLBuilder'''
                    
                    echo "Zipping build..."
                    bat '''7z a -tzip -r "%output%\\%WEBGL_BUILD_NAME%" "%CD%\\%output%\\%outputWebGLFolder%\\*"'''
                        
                    echo "Uploading release build artifact to Nexus..."                        
                    bat '''"%CURL_EXECUTABLE%" -X POST "http://localhost:8081/service/rest/v1/components?repository=%PROJECT_NAME%-Main" -H "accept: application/json" -H "Authorization: Basic %NEXUS_CREDENTIALS%" -F "raw.directory=WebGL" -F "raw.asset1=@%output%\\%WEBGL_BUILD_NAME%.zip;type=application/x-zip-compressed" -F "raw.asset1.filename=%WEBGL_BUILD_NAME%.zip"'''
                }
            }
        }
    }
    post {
            always {
                    script {
                        if(fileExists(output)) {
                            echo "Cleaning up workspace..."
                            bat '''RMDIR %output% /S /Q'''
                        }
                    }
                    
                    slackSend color: COLOR_MAP[currentBuild.currentResult],
                    message: "*${currentBuild.currentResult}:* Job ${env.JOB_NAME} build ${env.BUILD_NUMBER}\n More info at: ${env.BUILD_URL}"
            }
        }
}