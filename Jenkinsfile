pipeline {
    agent any

    options {
        timestamps()
        disableConcurrentBuilds()
    }

    environment {
        IMAGE_NAME = 'dialmock'
        IMAGE_TAG = "${env.BUILD_NUMBER}"
    }

    stages {
        stage('Checkout') {
            steps {
                checkout scm
                sh 'chmod +x scripts/*.sh'
            }
        }

        stage('Build + Test in Docker') {
            steps {
                sh './scripts/ci-build.sh'
                sh './scripts/ci-test.sh'
            }
            post {
                always {
                    archiveArtifacts artifacts: 'TestResults/**/*', fingerprint: true
                }
            }
        }

        /*
        stage('Push Docker Image') {
            when {
                branch 'main'
            }
            steps {
                withCredentials([usernamePassword(
                    credentialsId: 'docker-registry-creds',
                    usernameVariable: 'DOCKER_USERNAME',
                    passwordVariable: 'DOCKER_PASSWORD'
                )]) {
                    withEnv([
                        'DOCKER_REGISTRY=your-registry.example.com'
                    ]) {
                        sh './scripts/ci-docker-push.sh'
                    }
                }
            }
        }
        */

        /*
        stage('Deploy Local Container') {
            when {
                branch 'main'
            }
            steps {
                sh '''
                    docker stop dialmock || true
                    docker rm dialmock || true

                    docker run -d \
                      --name dialmock \
                      -p 8080:8080 \
                      ${IMAGE_NAME}:${IMAGE_TAG}
                '''
            }
        }
        */
    }

    post {
        success {
            echo 'Docker-only CI pipeline completed successfully.'
        }
        failure {
            echo 'Docker-only CI pipeline failed.'
        }
        always {
            cleanWs()
        }
    }
}