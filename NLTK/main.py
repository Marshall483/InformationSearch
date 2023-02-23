import os
import nltk
from nltk.corpus import stopwords
from nltk.stem import  SnowballStemmer

#nltk.download("punkt")
#nltk.download('averaged_perceptron_tagger')

RESULTS_DIR_PATH = '../Google/bin/Debug/net6.0/Results/13/'
RESULTS = 'Results'

def get_files_from_results():
     if os.path.exists(RESULTS_DIR_PATH):
         return os.listdir(RESULTS_DIR_PATH)

     return []

# Press the green button in the gutter to run the script.
if __name__ == '__main__':
    snowball = SnowballStemmer("russian")
    files = get_files_from_results()
    results_folder = os.getcwd() + os.sep + RESULTS + os.sep

    for path in files:
        text = open(RESULTS_DIR_PATH + path, "r", encoding="UTF-8").read()

        tokens = nltk.word_tokenize(text, language='Russian')
        tokens = [word.lower() for word in tokens if word.isalpha()]
        tokens = [word for word in tokens if not word in stopwords.words("russian")]
        tokens = [snowball.stem(word) for word in tokens]

        with open(results_folder + path, 'w', encoding='utf-8') as f:
            f.write(" ".join(tokens))

        print(path)


