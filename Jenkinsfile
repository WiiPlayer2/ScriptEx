node('docker') {
    checkout scm;

    env.DOTNET_CLI_HOME = "/tmp/DOTNET_CLI_HOME"
    env.DOTNET_NOLOGO = "true"
    
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

    stage('Check Integrity') {
        if(!changeRequest()) return;

        if(env.CHANGE_TARGET == 'main' && !(env.CHANGE_BRANCH ==~ /(release|hotfix)\/.+/)) {
            error('Only release and hotifx branches are allowed.')
        }
        if(env.CHANGE_TARGET == 'dev' && !(env.CHANGE_BRANCH ==~ /(feature|bug|hotfix)\/.+/)) {
            error('Only feature, bug and hotfix branches are allowed.')
        }
    }

    stage('Build') {
        if(!forceBuild && env.BUILD_NUMBER != '1') return;

        project.Build();
        built_app = true;
    }

    stage('Publish') {
        if(env.BRANCH_NAME !=~ /main|dev/) return;
        if(!built_app) return;

        project.Publish();
    }
}
