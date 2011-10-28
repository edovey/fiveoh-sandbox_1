//
//  TherapyView.h
//  BDViewer
//
//  Created by Liz Dovey on 11-10-20.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import <UIKit/UIKit.h>

@interface TherapyView : UIViewController <UIWebViewDelegate>

@property (retain, nonatomic) IBOutlet UIWebView *dataWebView;
@property (retain, nonatomic) NSString *presentationId;
@property (nonatomic, retain) NSString *presentationName;
@property (retain, nonatomic) NSString *diseaseId;
@property (retain, nonatomic) NSString *diseaseName;
@property (retain, nonatomic) NSString *detailHTMLString;

-(id)initWithDiseaseId:(NSString *)pDiseaseId withDiseaseName:(NSString *)pDiseaseName;
-(id)initWithPresentationId:(NSString *)pPresentationId withPresentationName:(NSString *) pPresentationName;
@end
