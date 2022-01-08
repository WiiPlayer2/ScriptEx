def imageName = 'script-ex';
def projectName = 'ScriptEx.Core';

def built_app = false;
def isTriggeredByIndexing = currentBuild.getBuildCauses('jenkins.branch.BranchIndexingCause').size();
def isTriggeredByCommit = currentBuild.getBuildCauses('com.cloudbees.jenkins.GitHubPushCause').size();
def isTriggeredByUser = currentBuild.getBuildCauses('hudson.model.Cause$UserIdCause').size();
def lastBuildFailed = "${currentBuild.previousBuild?.result}" != "SUCCESS";
def forceBuild = isTriggeredByUser || lastBuildFailed;

pipeline {
    agent {
        label 'docker'
    }

    environment {
        CLEAN_GIT_BRANCH = "${env.GIT_BRANCH.replaceAll('/', '_')}"
        DOTNET_CLI_HOME = "/tmp/DOTNET_CLI_HOME"
        DOTNET_NOLOGO = "true"
    }

    stages {
        stage('Check Integrity') {
            when { changeRequest() }
            steps {
                script {
                    if(env.CHANGE_TARGET == 'main' && !(env.CHANGE_BRANCH ==~ /(release|hotfix)\/.+/)) {
                        error('Only release and hotifx branches are allowed.')
                    }
                    if(env.CHANGE_TARGET == 'dev' && !(env.CHANGE_BRANCH ==~ /(feature|bug|hotfix)\/.+/)) {
                        error('Only feature, bug and hotfix branches are allowed.')
                    }
                }
            }
        }

        stage('Build') {
            failFast true
            parallel {
                stage('Build App') {
                    when {
                        anyOf {
                            expression { forceBuild }
                            environment name: 'BUILD_NUMBER', value: '1'
                        }
                    }
                    steps {
                        sh "docker build -t registry.dark-link.info/$imageName:$CLEAN_GIT_BRANCH -f ./$projectName/Dockerfile ."
                        script {
                            built_app = true;
                        }
                    }
                }
            }
        }

        stage('Publish') {
            when { branch pattern: "main|dev", comparator: "REGEXP" }
            parallel {
                stage('Publish App') {
                    when { expression { built_app } }
                    steps {
                        withDockerRegistry([credentialsId: 'vserver-container-registry', url: "https://registry.dark-link.info/"]) {
                            sh "docker tag registry.dark-link.info/$imageName:$CLEAN_GIT_BRANCH registry.dark-link.info/$imageName:latest"
                            sh "docker image push registry.dark-link.info/$imageName:$CLEAN_GIT_BRANCH"
                            sh "docker image push registry.dark-link.info/$imageName:latest"
                        }
                    }
                }
            }
        }
    }
}
