def dockerBuild = load "ci/jenkins/dockerBuild.groovy";
def causes = load "ci/jenkins/buildCauses.groovy";

def project = dockerBuild.createProject([
    imageName: 'script-ex',
    tag: env.BRANCH_NAME.replaceAll('/', '_'),
    registry: 'registry.dark-link.info',
    registryCredentials: 'vserver-container-registry',
    dockerfile: './ScriptEx.Core/Dockerfile',
]);

def built_app = false;
def lastBuildFailed = "${currentBuild.previousBuild?.result}" != "SUCCESS";
def forceBuild = causes.isTriggeredByUser || lastBuildFailed;

pipeline {
    agent {
        label 'docker'
    }

    environment {
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
                        script {
                            project.Build();
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
                        script {
                            project.Publish();
                        }
                    }
                }
            }
        }
    }
}
