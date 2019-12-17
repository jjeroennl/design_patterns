pipeline { 
    agent any 
    stages {
        stage('Require packages') { 
            steps { 
                sh 'cd idetector/idetector  && dotnet restore' 
                sh 'cd idetector/xUnitTest  && dotnet restore' 
                sh 'cd idetector/xUnitTest  && dotnet add package XunitXml.TestLogger --version 2.1.26' 
                sh 'cd idetector/xUnitTest  && dotnet tool install -g dotnet-xunit-to-junit || true' 
            }
        }
        stage('Build') { 
            steps { 
                sh 'rm -rf idetector/vs-plugin ' 
                sh 'cp /home/jenkinsj/actualsln.sln idetector/idetector.sln'
                sh 'cd idetector && dotnet build' 
            }
        }
        stage('Test'){
            steps {
                sh 'cd idetector/xUnitTest && dotnet test --logger:xunit'
                sh 'dotnet xunit-to-junit "idetector/xUnitTest/TestResults/TestResults.xml" "idetector/xUnitTest/TestResults/Testresults.junit.xml"'
                junit ' idetector/xUnitTest/TestResults/*.junit.xml' 
            }
        }
    }
}