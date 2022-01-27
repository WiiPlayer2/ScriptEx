node('docker') {
    checkout scm;

    env.DOTNET_CLI_HOME = "/tmp/DOTNET_CLI_HOME"
    env.DOTNET_NOLOGO = "true"
    
    def dockerBuild = load "ci/jenkins/dockerBuild.groovy";
    def causes = load "ci/jenkins/buildCauses.groovy";
    def gitFlow = load "ci/jenkins/gitFlow.groovy";

    def project = [
        imageName: 'script-ex',
        tag: env.BRANCH_NAME.replaceAll('/', '_'),
        registry: 'registry.dark-link.info',
        registryCredentials: 'vserver-container-registry',
        dockerfile: './ScriptEx.Core/Dockerfile',
    ];

    def built_app = false;
    def lastBuildFailed = "${currentBuild.previousBuild?.result}" != "SUCCESS";
    def forceBuild = causes.isTriggeredByUser || lastBuildFailed;

    gitFlow.checkPullRequest();

    stage('Build') {
        dockerBuild.build(project);
        built_app = true;
    }

    stage('Publish') {
        if(env.BRANCH_NAME !=~ /main|dev/) return;
        if(!built_app) return;

        dockerBuild.publish(project);
    }
}
