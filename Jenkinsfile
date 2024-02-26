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
        WINDOWS_PR_BUILD_NAME = "Windows-${env.BRANCH_NAME}-${currentBuild.number}"
        MACOS_BUILD_NAME = "MacOS-${currentBuild.number}"
        MACOS_DEV_BUILD_NAME = "MacOS-Dev-${currentBuild.number}"
        
        String output = "Output"
        String outputMacOSDevFolder = "CurrentMacDevBuild"
        String outputWindowsDevFolder = "CurrentWinDevBuild"
        String outputMacOSFolder = "CurrentMacBuild"
        String outputWindowsFolder = "CurrentWinBuild"
        
        NEXUS_CREDENTIALS = credentials('NEXUS_CREDENTIALS')
        OPENWEATHER_API_KEY = credentials('OPENWEATHER_API_KEY')
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
        
        stage('Pre Build Instructions') {
            steps {
                script {
                    
                    echo "Injecting OpenWeatherApiKey in the build"
                    bat '''if not exist ".\\Assets\\Secrets\\" mkdir ".\\Assets\\Secrets\\"'''
                    bat '''echo %OPENWEATHER_API_KEY% > ".\\Assets\\Secrets\\OpenWeatherAPIKey.txt"'''
                    
                }
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
                    
                    echo "Zipping build..."
                    bat '''7z a -tzip -r "%output%\\%WINDOWS_PR_BUILD_NAME%" "%CD%\\%output%\\%outputWindowsDevFolder%\\*"'''

                    echo "Uploading dev build artifact to Nexus..."
                    bat '''"%CURL_EXECUTABLE%" -X POST "http://localhost:8081/service/rest/v1/components?repository=%PROJECT_NAME%-PR" -H "accept: application/json" -H "Authorization: Basic %NEXUS_CREDENTIALS%" -F "raw.directory=Windows" -F "raw.asset1=@%output%\\%WINDOWS_PR_BUILD_NAME%.zip;type=application/x-zip-compressed" -F "raw.asset1.filename=%WINDOWS_PR_BUILD_NAME%.zip"'''
                    
                    echo "Launching MacOS Development Build..."
                    bat '''"%UNITY_EXECUTABLE%" -projectPath "%CD%" -quit -batchmode -nographics -customBuildPath "%CD%\\%output%\\%outputMacOSDevFolder%\\%PROJECT_NAME%.exe" -customBuildName "%PROJECT_NAME%" -executeMethod BuildUtility.MacOSDevBuilder'''
                    
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
                }
            }
        }
    }
    post {
            always {
                    script {
                        if(fileExists(output)) {
                            echo "Cleaning up output folder..."
                            bat '''RMDIR %output% /S /Q'''
                        }
                        //echo "Cleaning up workspace..."
                        //bat '''del /f /q *.*'''
                        //bat '''for /d %%D in (*) do (rmdir /s /q "%%D")'''
                    }
                    
                    slackSend color: COLOR_MAP[currentBuild.currentResult],
                    message: "*${currentBuild.currentResult}:* Job ${env.JOB_NAME} build ${env.BUILD_NUMBER}\n More info at: ${env.BUILD_URL}"
            }
        }
}
