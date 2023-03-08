#include <Arduino.h>
#define CHANNEL     0b00000001
#define HIGH_THRES   570
#define LOW_THRES   450

typedef struct Stats
{
  int sensorVal = 0, maxVal = 0, minVal = 1023, globalmaxVal = 0, globalminVal = 1023;  
}stats;

void configureADC(void);
void updateStats(int &val, int &min, int &max);
void printStats(int val, int min, int max);
void resetStats(int &val, int &min, int &max);
void updateStats2(stats *);
void printStats2(stats *);
void resetStats2(stats *);

float EMA_a_low = 0.3;    //initialization of EMA alpha
float EMA_a_high = 0.4;

int EMA_S_low = 0;        //initialization of EMA S
int EMA_S_high = 0;

int highpass = 0;
int bandpass = 0;

volatile unsigned int adc_res = 0;
volatile byte low;
volatile byte high;
bool conv_done = false;
//variable to store the value read
int sensorVal = 0, maxVal = 0, minVal = 1023, globalmaxVal = 0, globalminVal = 1023;
unsigned long start_time = 0;


stats s1;

void setup() {
  Serial.begin(115200);
  configureADC();

  // Start ADC Conversion
  ADCSRA |= (1 << ADSC);
  // put your setup code here, to run once:
}

void loop() {
  // put your main code here, to run repeatedly:
  if(conv_done == true)
  {
    conv_done = false;
    
    sensorVal = adc_res;
    // sensorVal = analogRead(A0);
    // Serial.println("Hello");
    EMA_S_low = (EMA_a_low*sensorVal) + ((1-EMA_a_low)*EMA_S_low);  //run the EMA
    EMA_S_high = (EMA_a_high*sensorVal) + ((1-EMA_a_high)*EMA_S_high);
   
    highpass = sensorVal - EMA_S_low;     //find the high-pass as before (for comparison)
    bandpass = EMA_S_high - EMA_S_low;      //find the band-pass

    s1.sensorVal = bandpass + sensorVal;

    updateStats2(&s1);
    printStats2(&s1);

    if(millis() - start_time > 5000)
    {
      start_time = millis();
      resetStats2(&s1);
    }

  }
}

void updateStats2(stats *s)
{
  s->minVal = (s->sensorVal < s->minVal) ? s->sensorVal : s->minVal;
  s->maxVal = (s->sensorVal > s->maxVal) ? s->sensorVal : s->maxVal;
  s->globalminVal = (s->sensorVal < s->globalminVal) ? s->sensorVal : s->globalminVal;
  s->globalmaxVal = (s->sensorVal > s->globalmaxVal) ? s->sensorVal : s->globalmaxVal;
}

void resetStats2(stats *s)
{
  s->minVal = 1023, s->maxVal = 0, s->sensorVal = 0;
}

void printStats2(stats *s)
{
  Serial.print(s->globalminVal);
  Serial.print("\t");
  Serial.print(s->minVal);
  Serial.print("\t");
  Serial.print(s->sensorVal);
  Serial.print("\t");
  Serial.print(s->maxVal);
  Serial.print("\t");
  Serial.print(s->globalmaxVal);
  Serial.print("\t");
  Serial.print(LOW_THRES);
  Serial.print("\t");
  Serial.print(HIGH_THRES);
  Serial.println("\t");
}

void updateStats(int &val, int &min, int &max)
{
  if(val < min)
    min = val;
  if(val > max)
    max = val;
}

void resetStats(int &val, int &min, int &max)
{
  min = 1023, max = 0, val = 0;
}

void printStats(int val, int min, int max)
{
  Serial.print(min);
  Serial.print("\t");
  Serial.print(val);
  Serial.print("\t");
  Serial.print(max);
  Serial.println("\t");
}

void configureADC(void)
{
  ADMUX |= CHANNEL;

  ADMUX |= (1 << REFS0);
  ADMUX &= ~(1 << REFS1);

  ADMUX &= ~(1<<ADLAR);

  ADCSRA |= 0B00000111;     //Selecting Pre-scalar value 128
  
  //Enable interrupt
  ADCSRA |= (1 << ADIE);
  
  ADCSRB &= 0B11111000;
  
  
  //Enable free-running mode
  ADCSRA |= (1 << ADATE);
  
  ADCSRA |= (1 << ADEN);    //Enable ADC

  sei();
}

ISR(ADC_vect)
{
  low = ADCL;
  high = ADCH;
  adc_res = ((high << 8) | low);
  // adc_res = ADCW;
  conv_done = true;
}