﻿using java.io;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using weka.classifiers.bayes;
using weka.classifiers.meta;
using weka.core;
using weka.filters.unsupervised.attribute;
using WekaClassifier.Enums;
using WekaClassifier.Helper;


/*Notes for Project
    Generated WEKA.dll from weka.jar using IKVM and added to reference
    Added same version OF IKVM to Project that we used to generate weka.dll with
    Added MONO.C#
*/

namespace WekaClassifier
{


    /// <summary>
    /// Weka Classifier
    /// </summary>
    public class WekaClassifier
    {

        public string error { get; set; }

        private List<string> tempAllClassification = new List<string>();

        public string classification { get; set; }
        public string allOutputText { get; set; }

        public List<string> AllClassification { get; set; }





        /// <summary>
        /// This is the constructor for BOW single review classification. Overloaded Method
        /// </summary>
        /// <param name="singleReviewBOW"></param>
        public WekaClassifier(List<string> inputBoWList, string trainingFilePath, string directoryName, ClassifierName classifierName, TextFilterType textFilterType, ClassificationScheme classificationScheme = ClassificationScheme.MultiClass)
        {
            if (classificationScheme == ClassificationScheme.Binary)
            {
                ConstructBinaryBOWArffFile(inputBoWList, directoryName);
                switch (classifierName)
                {
                    case ClassifierName.SupportVectorMachine:
                        BinarySVM(trainingFilePath, directoryName, textFilterType);
                        break;
                    case ClassifierName.NaiveBayes:
                        BinaryNaiveBayes(trainingFilePath, directoryName, textFilterType);
                        break;
                    default:
                        break;
                }
            }
            else
            {
                ConstructBOWArffFile(inputBoWList, directoryName);
                switch (classifierName)
                {
                    case ClassifierName.SupportVectorMachine:
                        FilteredSVM("BOW", trainingFilePath, directoryName, textFilterType);
                        break;
                    case ClassifierName.NaiveBayes:
                        FilteredNaiveBayes("BOW", trainingFilePath, directoryName, textFilterType);
                        break;
                    default:
                        break;
                }
            }
        }




        private void ConstructBinaryBOWArffFile(List<string> inputBoWList, string directoryName)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);


            var testDatatsetFilePath = specificFolder + "\\InputData\\TrainingDatasets\\Binary NFR Test.arff";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(testDatatsetFilePath))
            {
                file.WriteLine("@relation Test");
                file.WriteLine();
                file.WriteLine("@attribute text string");
                file.WriteLine("@attribute @@class@@ {NFR,Mis}");
                file.WriteLine();
                file.WriteLine("@data");

                foreach (string line in inputBoWList)
                {
                    file.WriteLine("'" + line + " " + "',?");
                }
                file.Close();
            }
        }

        /// <summary>
        /// This is the constructor for BOF  user reviews. Overloaded Method
        /// </summary>
        /// <param name="inputFramesList"></param>
        public WekaClassifier(List<List<string>> inputBoFList, string trainingFilePath, string directoryName, ClassifierName classifierName, TextFilterType textFilterType)
        {
            ConstructFramesArffFile(inputBoFList, directoryName);
            switch (classifierName)
            {
                case ClassifierName.SupportVectorMachine:
                    FilteredSVM("BOF", trainingFilePath, directoryName, textFilterType);
                    break;
                case ClassifierName.NaiveBayes:
                    FilteredNaiveBayes("BOF", trainingFilePath, directoryName, textFilterType);
                    break;
                case ClassifierName.RandomForest:
                    break;
                default:
                    break;
            }
        }

        /// <summary>
        /// Method to construct BOW Arff File. Input is the list of strings of BOWs
        /// </summary>
        /// <param name="inputBOW"></param>
        private void ConstructBOWArffFile(List<string> inputBOW, string directoryName)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);


            var testDatatsetFilePath = specificFolder + "\\InputData\\TrainingDatasets\\Test.arff";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(testDatatsetFilePath))
            {
                file.WriteLine("@relation Test");
                file.WriteLine();
                file.WriteLine("@attribute text string");
                file.WriteLine("@attribute @@class@@ {BugReport,FeatureRequest,Other}");
                file.WriteLine();
                file.WriteLine("@data");

                foreach (string line in inputBOW)
                {
                    file.WriteLine("'" + line + " " + "',?");
                }
                file.Close();
            }
        }

        /// <summary>
        /// This is the method to construct BOF Arff file. Input is list of list of strings
        /// </summary>
        /// <param name="inputFrames"></param>
        private void ConstructFramesArffFile(List<List<string>> inputFrames, string directoryName)
        {

            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var testDatatsetFilePath = specificFolder + "\\InputData\\TrainingDatasets\\Test.arff";
            using (System.IO.StreamWriter file =
            new System.IO.StreamWriter(testDatatsetFilePath))
            {
                file.WriteLine("@relation Test");
                file.WriteLine();
                file.WriteLine("@attribute text string");
                file.WriteLine("@attribute @@class@@ {BugReport,FeatureRequest,Other}");
                file.WriteLine();
                file.WriteLine("@data");

                foreach (List<string> line in inputFrames)
                {
                    file.WriteLine();
                    file.Write("'");
                    foreach (string data in line)
                    {
                        file.Write(data + " ");
                    }
                    file.Write("',?");
                }
            }
        }


        #region Binary Naive Bayes

        /// <summary>
        /// Binary Classification for NFR extraction using Naive Bayes
        /// </summary>
        /// <param name="trainingFilePath"></param>
        /// <param name="directoryName"></param>
        /// <param name="textFilterType"></param>
        private void BinaryNaiveBayes(string trainingFilePath, string directoryName, TextFilterType textFilterType)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            try
            {
                var trainingDatatsetFilePath = specificFolder + "\\InputData\\TrainingDatasets\\Binary NFR Training.arff"; ;
                var testDatasetFilePath = specificFolder + "\\InputData\\TrainingDatasets\\Binary NFR Test.arff";

                //User Supplied Custom Training File
                if (trainingFilePath != null)
                {
                    trainingDatatsetFilePath = trainingFilePath;
                }

                java.io.BufferedReader trainReader = new BufferedReader(new FileReader(trainingDatatsetFilePath)); //File with text examples
                BufferedReader classifyReader = new BufferedReader(new FileReader(testDatasetFilePath)); //File with text to classify

                Instances trainInsts = new Instances(trainReader);
                Instances classifyInsts = new Instances(classifyReader);

                trainInsts.setClassIndex(trainInsts.numAttributes() - 1);

                classifyInsts.setClassIndex(classifyInsts.numAttributes() - 1);

                FilteredClassifier model = new FilteredClassifier();

                StringToWordVector stringtowordvector = new StringToWordVector();
                stringtowordvector.setTFTransform(true);
                model.setFilter(new StringToWordVector());
                model.setClassifier(new NaiveBayes());

                bool exists;
                var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());

                //Pointig to appdata folder
                directoryRoot = specificFolder;
                //Check if the model exists and if not then build a model
                switch (textFilterType)
                {
                    //Case No Filter
                    case TextFilterType.NoFilter:
                        exists = BinaryNBNoFilterCheckifModelExists(trainingDatatsetFilePath);
                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\BinaryNBNoFilterModel.dat", model);

                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\NB\\BinaryNBNoFilterFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\BinaryNBNoFilterModel.dat");
                        }
                        break;


                    //Case Stopwords Removal
                    case TextFilterType.StopwordsRemoval:
                        exists = BinaryNBSWRCheckifModelExists(trainingDatatsetFilePath);
                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\BinaryNBSWRFilterModel.dat", model);
                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\NB\\BinaryNBSWRFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\BinaryNBSWRFilterModel.dat");
                        }
                        break;

                    //Case Stemming
                    case TextFilterType.Stemming:
                        exists = BinaryNBSTCheckifModelExists(trainingDatatsetFilePath);
                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\BinaryNBSTFilterModel.dat", model);
                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\NB\\BinaryNBSTFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\BinaryNBSTFilterModel.dat");
                        }
                        break;

                    //Case Stopwords Removal with Stemming
                    case TextFilterType.StopwordsRemovalStemming:
                        exists = BinaryNBSWRSTCheckifModelExists(trainingDatatsetFilePath);
                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\BinaryNBSWRSTFilterModel.dat", model);
                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\NB\\BinaryNBSWRSTFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\BinaryNBSWRSTFilterModel.dat");
                        }
                        break;
                    default:
                        break;
                }
                for (int i = 0; i < classifyInsts.numInstances(); i++)
                {
                    classifyInsts.instance(i).setClassMissing();
                    double cls = model.classifyInstance(classifyInsts.instance(i));
                    classifyInsts.instance(i).setClassValue(cls);
                    classification = cls == 0 ? "NFR"
                                    : "Mis";
                    tempAllClassification.Add(classification);
                }
                AllClassification = tempAllClassification;
            }
            catch (Exception o)
            {
                error = o.ToString();
            }

        }

        /// <summary>
        /// Check if Binary Naive Bayes model for stopword removal with stemming already exists
        /// </summary>
        /// <returns></returns>
        private bool BinaryNBSWRSTCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;

            var folder = directoryRoot + @"\Model\NB";
            if (System.IO.File.Exists(folder + @"\BinaryNBSWRSTFilterModel.dat") && System.IO.File.Exists(folder + @"\BinaryNBSWRSTFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\BinaryNBSWRSTFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if Binary Naive Bayes model for stemming already exists
        /// </summary>
        /// <returns></returns>
        private bool BinaryNBSTCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;
            var folder = directoryRoot + @"\Model\NB";
            if (System.IO.File.Exists(folder + @"\BinaryNBSTFilterModel.dat") && System.IO.File.Exists(folder + @"\BinaryNBSTFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\BinaryNBSTFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if Binary Naive Bayes model for stopwords removal already exists
        /// </summary>
        /// <returns></returns>
        private bool BinaryNBSWRCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;
            var folder = directoryRoot + @"\Model\NB";
            if (System.IO.File.Exists(folder + @"\BinaryNBSWRFilterModel.dat") && System.IO.File.Exists(folder + @"\BinaryNBSWRFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\BinaryNBSWRFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if Binary Naive Bayes model for no filter already exists
        /// </summary>
        /// <returns></returns>
        private bool BinaryNBNoFilterCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;
            var folder = directoryRoot + @"\Model\NB";
            if (System.IO.File.Exists(folder + @"\BinaryNBNoFilterModel.dat") && System.IO.File.Exists(folder + @"\BinaryNBNoFilterFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\BinaryNBNoFilterFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        #endregion Binary Naive Bayes

        #region Binary SVM

        /// <summary>
        /// Binary Classification for NFR extraction using SVM
        /// </summary>
        /// <param name="trainingFilePath"></param>
        /// <param name="directoryName"></param>
        /// <param name="textFilterType"></param>
        private void BinarySVM(string trainingFilePath, string directoryName, TextFilterType textFilterType)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            try
            {
                var trainingDatatsetFilePath = specificFolder + "\\InputData\\TrainingDatasets\\Binary NFR Training.arff"; ;
                var testDatasetFilePath = specificFolder + "\\InputData\\TrainingDatasets\\Binary NFR Test.arff";

                //User Supplied Custom Training File
                if (trainingFilePath != null)
                {
                    trainingDatatsetFilePath = trainingFilePath;
                }

                java.io.BufferedReader trainReader = new BufferedReader(new FileReader(trainingDatatsetFilePath));//File with text examples
                BufferedReader classifyReader = new BufferedReader(new FileReader(testDatasetFilePath));//File with text to classify

                Instances trainInsts = new Instances(trainReader);
                Instances classifyInsts = new Instances(classifyReader);

                trainInsts.setClassIndex(trainInsts.numAttributes() - 1);
                classifyInsts.setClassIndex(classifyInsts.numAttributes() - 1);

                FilteredClassifier model = new FilteredClassifier();

                StringToWordVector stringtowordvector = new StringToWordVector();
                stringtowordvector.setTFTransform(true);
                model.setFilter(new StringToWordVector());

                weka.classifiers.Classifier smocls = new weka.classifiers.functions.SMO();

                //smocls.setOptions(weka.core.Utils.splitOptions("-C 1.0 -L 0.001 -P 1.0E-12 -N 0 -V -1 -W 1 -K \"weka.classifiers.functions.supportVector.Puk -C 250007 -O 1.0 -S 1.0\""));
                smocls.setOptions(weka.core.Utils.splitOptions("-C 1.0 -L 0.0010 -P 1.0E-12 -N 0 -V -1 -W 1 -K \"weka.classifiers.functions.supportVector.PolyKernel -C 250007 -E 1.0\""));
                model.setClassifier(smocls);

                bool exists;
                var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
                directoryRoot = specificFolder;
                //Check if the model exists and if not then build a model
                switch (textFilterType)
                {
                    case TextFilterType.NoFilter:
                        exists = BinarySVMNoFilterCheckifModelExists(trainingDatatsetFilePath);

                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\BinarySVMNoFilterModel.dat", model);
                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\SVM\\BinarySVMNoFilterFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\BinarySVMNoFilterModel.dat");
                        }

                        break;

                    //Case Stopwords Removal
                    case TextFilterType.StopwordsRemoval:
                        exists = BinarySVMSWRCheckifModelExists(trainingDatatsetFilePath);
                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\BinarySVMSWRFilterModel.dat", model);
                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\SVM\\BinarySVMSWRFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\BinarySVMSWRFilterModel.dat");
                        }

                        break;

                    //Case Stemming
                    case TextFilterType.Stemming:
                        exists = BinarySVMSTCheckifModelExists(trainingDatatsetFilePath);
                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\BinarySVMSTFilterModel.dat", model);
                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\SVM\\BinarySVMSTFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\BinarySVMSTFilterModel.dat");
                        }
                        break;

                    //Case Stopwords Removal with Stemming
                    case TextFilterType.StopwordsRemovalStemming:
                        exists = BinarySVMSWRSTCheckifModelExists(trainingDatatsetFilePath);
                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\BinarySVMSWRSTFilterModel.dat", model);
                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\SVM\\BinarySVMSWRSTFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\BinarySVMSWRSTFilterModel.dat");
                        }
                        break;
                    default:
                        break;
                }

                //model.buildClassifier(trainInsts);
                for (int i = 0; i < classifyInsts.numInstances(); i++)
                {
                    classifyInsts.instance(i).setClassMissing();
                    double cls = model.classifyInstance(classifyInsts.instance(i));
                    classifyInsts.instance(i).setClassValue(cls);
                    classification = cls == 0 ? "NFR"
                                    : "Mis";
                    tempAllClassification.Add(classification);
                }
                AllClassification = tempAllClassification;
            }
            catch (Exception o)
            {
                error = o.ToString();
            }
        }


        /// <summary>
        /// Check if Binary SVM model for Stopwords Removal Stemming Exists
        /// </summary>
        /// <returns></returns>
        private bool BinarySVMSWRSTCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;

            var folder = directoryRoot + @"\Model\SVM";
            if (System.IO.File.Exists(folder + @"\BinarySVMSWRSTFilterModel.dat") && System.IO.File.Exists(folder + @"\BinarySVMSWRSTFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\BinarySVMSWRSTFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if Binary SVM model for Stemming Exists
        /// </summary>
        /// <returns></returns>
        private bool BinarySVMSTCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;

            var folder = directoryRoot + @"\Model\SVM";
            if (System.IO.File.Exists(folder + @"\BinarySVMSTFilterModel.dat") && System.IO.File.Exists(folder + @"\BinarySVMSTFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\BinarySVMSTFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if Binary SVM model for Stopwords Removal Exists
        /// </summary>
        /// <returns></returns>
        private bool BinarySVMSWRCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;

            var folder = directoryRoot + @"\Model\SVM";
            if (System.IO.File.Exists(folder + @"\BinarySVMSWRFilterModel.dat") && System.IO.File.Exists(folder + @"\BinarySVMSWRFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\BinarySVMSWRFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if Binary SVM model for no Filter exists
        /// </summary>
        /// <returns></returns>
        private bool BinarySVMNoFilterCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;

            var folder = directoryRoot + @"\Model\SVM";
            if (System.IO.File.Exists(folder + @"\BinarySVMNoFilterModel.dat") && System.IO.File.Exists(folder + @"\BinarySVMNoFilterFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\BinarySVMNoFilterFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        #endregion Binary SVM

        #region Multi Class Naive Bayes

        /// <summary>
        /// Filtered Naive Bayes Classification with type specified. i.e. BOF or BOW
        /// </summary>
        /// <param name="type"></param>
        private void FilteredNaiveBayes(string type, string trainingFilePath, string directoryName, TextFilterType textFilterType)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);


            try
            {
                var trainingDatatsetFilePath = "";
                if (type == "BOF")
                {
                    trainingDatatsetFilePath = specificFolder + "\\InputData\\TrainingDatasets\\BOF Dataset.arff";
                }
                else
                {
                    trainingDatatsetFilePath = specificFolder + "\\InputData\\TrainingDatasets\\BOW Dataset.arff";
                }

                var testDatasetFilePath = specificFolder + "\\InputData\\TrainingDatasets\\Test.arff";

                if (trainingFilePath != null)
                {
                    trainingDatatsetFilePath = trainingFilePath;
                }

                java.io.BufferedReader trainReader = new BufferedReader(new FileReader(trainingDatatsetFilePath));//File with text examples
                BufferedReader classifyReader = new BufferedReader(new FileReader(testDatasetFilePath));//File with text to classify

                Instances trainInsts = new Instances(trainReader);
                Instances classifyInsts = new Instances(classifyReader);

                trainInsts.setClassIndex(trainInsts.numAttributes() - 1);

                classifyInsts.setClassIndex(classifyInsts.numAttributes() - 1);

                FilteredClassifier model = new FilteredClassifier();

                StringToWordVector stringtowordvector = new StringToWordVector();
                stringtowordvector.setTFTransform(true);
                model.setFilter(new StringToWordVector());
                model.setClassifier(new NaiveBayes());

                bool exists;
                var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());

                //Pointig to appdata folder
                directoryRoot = specificFolder;
                //Check if the model exists and if not then build a model
                switch (textFilterType)
                {
                    //Case No Filter
                    case TextFilterType.NoFilter:
                        exists = NBNoFilterCheckifModelExists(trainingDatatsetFilePath);
                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\NBNoFilterModel.dat", model);

                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\NB\\NBNoFilterFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\NBNoFilterModel.dat");
                        }
                        break;


                    //Case Stopwords Removal
                    case TextFilterType.StopwordsRemoval:
                        exists = NBSWRCheckifModelExists(trainingDatatsetFilePath);
                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\NBSWRFilterModel.dat", model);
                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\NB\\NBSWRFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\NBSWRFilterModel.dat");
                        }
                        break;

                    //Case Stemming
                    case TextFilterType.Stemming:
                        exists = NBSTCheckifModelExists(trainingDatatsetFilePath);
                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\NBSTFilterModel.dat", model);
                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\NB\\NBSTFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\NBSTFilterModel.dat");
                        }
                        break;

                    //Case Stopwords Removal with Stemming
                    case TextFilterType.StopwordsRemovalStemming:
                        exists = NBSWRSTCheckifModelExists(trainingDatatsetFilePath);
                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\NBSWRSTFilterModel.dat", model);
                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\NB\\NBSWRSTFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\NB\NBSWRSTFilterModel.dat");
                        }
                        break;
                    default:
                        break;
                }
                for (int i = 0; i < classifyInsts.numInstances(); i++)
                {
                    classifyInsts.instance(i).setClassMissing();
                    double cls = model.classifyInstance(classifyInsts.instance(i));
                    classifyInsts.instance(i).setClassValue(cls);
                    classification = cls == 0 ? "Bug Report"
                                    : cls == 1 ? "Feature Request"
                                    : "Other";
                    tempAllClassification.Add(classification);
                }
                AllClassification = tempAllClassification;
            }
            catch (Exception o)
            {
                error = o.ToString();
            }
        }

        /// <summary>
        /// Check if Naive Bayes model for stopword removal with stemming already exists
        /// </summary>
        /// <returns></returns>
        private bool NBSWRSTCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;

            var folder = directoryRoot + @"\Model\NB";
            if (System.IO.File.Exists(folder + @"\NBSWRSTFilterModel.dat") && System.IO.File.Exists(folder + @"\NBSWRSTFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\NBSWRSTFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if Naive Bayes model for stemming already exists
        /// </summary>
        /// <returns></returns>
        private bool NBSTCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;
            var folder = directoryRoot + @"\Model\NB";
            if (System.IO.File.Exists(folder + @"\NBSTFilterModel.dat") && System.IO.File.Exists(folder + @"\NBSTFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\NBSTFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if Naive Bayes model for stopwords removal already exists
        /// </summary>
        /// <returns></returns>
        private bool NBSWRCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;
            var folder = directoryRoot + @"\Model\NB";
            if (System.IO.File.Exists(folder + @"\NBSWRFilterModel.dat") && System.IO.File.Exists(folder + @"\NBSWRFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\NBSWRFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if Naive Bayes model for no filter already exists
        /// </summary>
        /// <returns></returns>
        private bool NBNoFilterCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;
            var folder = directoryRoot + @"\Model\NB";
            if (System.IO.File.Exists(folder + @"\NBNoFilterModel.dat") && System.IO.File.Exists(folder + @"\NBNoFilterFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\NBNoFilterFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        #endregion Multi Class Naive Bayes

        #region Multi Class SVM

        /// <summary>
        /// Filtered Support Vector Machine Classification with type specified. i.e. BOF or BOW
        /// </summary>
        /// <param name="type"></param>
        private void FilteredSVM(string type, string trainingFilePath, string directoryName, TextFilterType textFilterType)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);



            try
            {
                var trainingDatatsetFilePath = "";
                if (type == "BOF")
                {
                    trainingDatatsetFilePath = specificFolder + "\\InputData\\TrainingDatasets\\BOF Dataset.arff";
                }
                else
                {
                    trainingDatatsetFilePath = specificFolder + "\\InputData\\TrainingDatasets\\BOW Dataset.arff";
                }

                var testDatasetFilePath = specificFolder + "\\InputData\\TrainingDatasets\\Test.arff";

                // If training file path is supplied then use it.
                if (trainingFilePath != null)
                {
                    trainingDatatsetFilePath = trainingFilePath;
                }

                java.io.BufferedReader trainReader = new BufferedReader(new FileReader(trainingDatatsetFilePath));//File with text examples
                BufferedReader classifyReader = new BufferedReader(new FileReader(testDatasetFilePath));//File with text to classify

                Instances trainInsts = new Instances(trainReader);
                Instances classifyInsts = new Instances(classifyReader);

                trainInsts.setClassIndex(trainInsts.numAttributes() - 1);
                classifyInsts.setClassIndex(classifyInsts.numAttributes() - 1);

                FilteredClassifier model = new FilteredClassifier();

                StringToWordVector stringtowordvector = new StringToWordVector();
                stringtowordvector.setTFTransform(true);
                model.setFilter(new StringToWordVector());

                weka.classifiers.Classifier smocls = new weka.classifiers.functions.SMO();

                //smocls.setOptions(weka.core.Utils.splitOptions("-C 1.0 -L 0.001 -P 1.0E-12 -N 0 -V -1 -W 1 -K \"weka.classifiers.functions.supportVector.Puk -C 250007 -O 1.0 -S 1.0\""));
                smocls.setOptions(weka.core.Utils.splitOptions("-C 1.0 -L 0.0010 -P 1.0E-12 -N 0 -V -1 -W 1 -K \"weka.classifiers.functions.supportVector.PolyKernel -C 250007 -E 1.0\""));
                model.setClassifier(smocls);

                bool exists;
                var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
                directoryRoot = specificFolder;
                //Check if the model exists and if not then build a model
                switch (textFilterType)
                {
                    case TextFilterType.NoFilter:
                        exists = SVMNoFilterCheckifModelExists(trainingDatatsetFilePath);

                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\SVMNoFilterModel.dat", model);
                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\SVM\\SVMNoFilterFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\SVMNoFilterModel.dat");
                        }

                        break;

                    //Case Stopwords Removal
                    case TextFilterType.StopwordsRemoval:
                        exists = SVMSWRCheckifModelExists(trainingDatatsetFilePath);
                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\SVMSWRFilterModel.dat", model);
                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\SVM\\SVMSWRFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\SVMSWRFilterModel.dat");
                        }

                        break;

                    //Case Stemming
                    case TextFilterType.Stemming:
                        exists = SVMSTCheckifModelExists(trainingDatatsetFilePath);
                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\SVMSTFilterModel.dat", model);
                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\SVM\\SVMSTFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\SVMSTFilterModel.dat");
                        }
                        break;

                    //Case Stopwords Removal with Stemming
                    case TextFilterType.StopwordsRemovalStemming:
                        exists = SVMSWRSTCheckifModelExists(trainingDatatsetFilePath);
                        //if does not exists then build model and save it and save the file also for current filter
                        if (!exists)
                        {
                            model.buildClassifier(trainInsts);
                            Helper.Helper.WriteToBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\SVMSWRSTFilterModel.dat", model);
                            string content = System.IO.File.ReadAllText(trainingDatatsetFilePath);
                            using (var sW = new StreamWriter(directoryRoot + @"\Model\SVM\\SVMSWRSTFile.dat"))
                            {
                                sW.Write(content);
                            }
                        }
                        // if exists then read the file and use the model
                        else
                        {
                            model = Helper.Helper.ReadFromBinaryFile<FilteredClassifier>(directoryRoot + @"\Model\SVM\SVMSWRSTFilterModel.dat");
                        }
                        break;
                    default:
                        break;
                }

                //model.buildClassifier(trainInsts);
                for (int i = 0; i < classifyInsts.numInstances(); i++)
                {
                    classifyInsts.instance(i).setClassMissing();
                    double cls = model.classifyInstance(classifyInsts.instance(i));
                    classifyInsts.instance(i).setClassValue(cls);
                    classification = cls == 0 ? "Bug Report"
                                    : cls == 1 ? "Feature Request"
                                    : "Other";
                    tempAllClassification.Add(classification);
                }
                AllClassification = tempAllClassification;
            }
            catch (Exception o)
            {
                error = o.ToString();
            }
        }

        /// <summary>
        /// Check if SVM model for Stopwords Removal Stemming Exists
        /// </summary>
        /// <returns></returns>
        private bool SVMSWRSTCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;

            var folder = directoryRoot + @"\Model\SVM";
            if (System.IO.File.Exists(folder + @"\SVMSWRSTFilterModel.dat") && System.IO.File.Exists(folder + @"\SVMSWRSTFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\SVMSWRSTFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if SVM model for Stemming Exists
        /// </summary>
        /// <returns></returns>
        private bool SVMSTCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;

            var folder = directoryRoot + @"\Model\SVM";
            if (System.IO.File.Exists(folder + @"\SVMSTFilterModel.dat") && System.IO.File.Exists(folder + @"\SVMSTFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\SVMSTFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if SVM model for Stopwords Removal Exists
        /// </summary>
        /// <returns></returns>
        private bool SVMSWRCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;

            var folder = directoryRoot + @"\Model\SVM";
            if (System.IO.File.Exists(folder + @"\SVMSWRFilterModel.dat") && System.IO.File.Exists(folder + @"\SVMSWRFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\SVMSWRFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Check if SVM model for no Filter exists
        /// </summary>
        /// <returns></returns>
        private bool SVMNoFilterCheckifModelExists(string trainingDatatsetFilePath)
        {
            var currDir = System.Environment.GetFolderPath(System.Environment.SpecialFolder.ApplicationData);

            // Combine the base folder with your specific folder....
            string specificFolder = System.IO.Path.Combine(currDir, "MARC 3.0");

            // Check if folder exists and if not, create it
            if (!Directory.Exists(specificFolder))
                Directory.CreateDirectory(specificFolder);

            var directoryRoot = System.IO.Path.GetDirectoryName(Directory.GetCurrentDirectory());
            directoryRoot = specificFolder;

            var folder = directoryRoot + @"\Model\SVM";
            if (System.IO.File.Exists(folder + @"\SVMNoFilterModel.dat") && System.IO.File.Exists(folder + @"\SVMNoFilterFile.dat"))
            {
                var isEqual = System.IO.File.ReadLines(folder + @"\SVMNoFilterFile.dat").SequenceEqual(System.IO.File.ReadLines(trainingDatatsetFilePath));
                return isEqual;
            }
            else
            {
                return false;
            }
        }

        #endregion Multi Class SVM
    }
}
