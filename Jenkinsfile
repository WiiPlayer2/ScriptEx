node('docker') {
    checkout scm;

    env.DOTNET_CLI_HOME = "/tmp/DOTNET_CLI_HOME"
    env.DOTNET_NOLOGO = "true"
    
    def dockerBuild = load "ci/jenkins/dockerBuild.groovy";
    def causes = load "ci/jenkins/buildCauses.groovy";
    def gitFlow = load "ci/jenkins/gitFlow.groovy";

    def project = dockerBuild.prepare([
        imageName: 'script-ex',
        tag: env.BRANCH_NAME.replaceAll('/', '_'),
        registry: 'registry.dark-link.info',
        registryCredentials: 'vserver-container-registry',
        dockerfile: './ScriptEx.Core/Dockerfile',
    ]);

    def lastBuildFailed = "${currentBuild.previousBuild?.result}" != "SUCCESS";
    def forceBuild = causes.isTriggeredByUser || lastBuildFailed;

    gitFlow.checkPullRequest();

    if(env.BRANCH_NAME ==~ /main|dev/)
    {
        dockerBuild.buildAndPublish(project);
    }
    else
    {
        stage('Build') {
            dockerBuild.build(project);
        }
    }
}
