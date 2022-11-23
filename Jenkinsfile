pipeline {
    agent any
    stages {
        stage ('Clean workspace') {
            steps {
                echo 'Clean Workspace'
                cleanWs()
            }
        }
        stage ('Git Checkout') {
            steps {
                echo 'Checkout the source code from Github'
                git branch: 'master', url: 'https://github.com/fjsevillamora/AccountOwnerServer.git'
            }
        }
        stage ('Restore NuGet Packages') {
            steps {
                echo 'Restoring NuGet Packages'
                bat 'dotnet restore AccountOwnerServer.sln'
            }
        }
        stage ('Clean Solution') {
            steps {
                echo 'Cleaning solution'
                bat 'dotnet clean AccountOwnerServer.sln'
            }
        }

        stage ('Build Solution') {
            steps {
                echo 'Building solution'
                bat 'dotnet build AccountOwnerServer.sln'
            }
        }

        stage ('Deploy Solution') {
            steps {
                echo 'Deploying solution to target Folder'
                echo 'Creating deploy folder in root'
                bat 'mkdir deploy'
                bat 'mkdir publishArtifacts'
                bat 'mkdir published'

                echo 'Publishing the solution'
                bat 'dotnet publish --self-contained --runtime  win-x64 -c Release  AccountOwnerServer.sln -o ./deploy'

                echo 'Create Zip file of deploy folder in publishArtifacts'
                fileOperations([fileZipOperation(folderPath: 'deploy', outputFolderPath: 'publishArtifacts')])
                echo 'Unzip file of deploy folder in published'
                fileOperations([fileUnZipOperation(filePath: 'publishArtifacts/deploy.zip', targetLocation: 'published')])
                echo 'Copy of publish files of published/deploy folder to published'
                fileOperations([folderCopyOperation(destinationFolderPath: 'published', sourceFolderPath: 'published/deploy')])
                echo 'Delete published/deploy folder'
                fileOperations([folderDeleteOperation('published/deploy')])
            }
        }
    }
}
