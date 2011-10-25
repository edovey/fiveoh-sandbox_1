//
//  TherapyView.m
//  BDViewer
//
//  Created by Liz Dovey on 11-10-20.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "TherapyView.h"
#import "BDTherapyGroup.h"
#import "BDTherapy.h"
#import "BDPathogenGroup.h"
#import "BDPathogen.h"
#import "BDTherapyGroup.h"
#import "BDTherapy.h"
#import "BDPresentation.h"

@interface TherapyView() 
-(void)retrieveOverviewForTherapy;
-(void)loadHTMLIntoWebView;
-(NSString *)buildHTMLFromData;
-(NSString *)buildPathogenGroupHTML:(BDPathogenGroup *)thePathogen;
-(NSString *)buildTherapyGroupHTML:(BDTherapyGroup *)theTherapyGroup;
-(NSString *)buildTherapyHTML:(BDTherapy *)theTherapy;

@end

@implementation TherapyView
@synthesize dataWebView;
@synthesize presentationId;
@synthesize presentationName;
@synthesize diseaseId;
@synthesize diseaseName;
@synthesize overviewHTMLString;
@synthesize detailHTMLString;

-(id)initWithPresentationId:(NSString *)pPresentationId withPresentationName:(NSString *)pPresentationName
{
    self = [super initWithNibName:@"TherapyView" bundle:nil];
    if(self)
    {
        self.presentationId = [pPresentationId retain];
        self.presentationName = [pPresentationName retain];
        self.title = presentationName;
    }
    return self;
}

-(id)initWithDiseaseId:(NSString *)pDiseaseId withDiseaseName:(NSString *) pDiseaseName
{
    self = [super initWithNibName:@"TherapyView" bundle:nil];
    if(self)
    {
        self.diseaseId = [pDiseaseId retain];
        self.diseaseName = [pDiseaseName retain];
        self.title = diseaseName;
        
        if(self.diseaseId != nil && [self.diseaseId length] > 0)
        {           
            // get presentation id using disease record
            NSArray *presentationArray = [BDPresentation retrieveAllWithParentUUID:diseaseId];
            self.presentationId = [[presentationArray objectAtIndex:0] uuid];
        }
    }
    return self;
}

- (void)didReceiveMemoryWarning
{
    // Releases the view if it doesn't have a superview.
    [super didReceiveMemoryWarning];
    
    // Release any cached data, images, etc that aren't in use.
}

#pragma mark - View lifecycle

- (void)viewDidLoad
{
    [super viewDidLoad];

    // Do any additional setup after loading the view from its nib.
}

-(void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    [self retrieveOverviewForTherapy];
    detailHTMLString = [NSString stringWithString:[self buildHTMLFromData]];
    [self loadHTMLIntoWebView];
}

- (void)viewDidUnload
{
    [dataWebView release];
    dataWebView = nil;
    overviewHTMLString = nil;
    detailHTMLString = nil;
    presentationId = nil;
    presentationName = nil;
    diseaseId = nil;
    diseaseName = nil;
    [super viewDidUnload];
    // Release any retained subviews of the main view.
    // e.g. self.myOutlet = nil;
}

- (BOOL)shouldAutorotateToInterfaceOrientation:(UIInterfaceOrientation)interfaceOrientation
{
    // Return YES for supported orientations
    return (interfaceOrientation == UIInterfaceOrientationPortrait);
}

- (void)dealloc {
//    [detailHTMLString release];
    [overviewHTMLString release];
    [presentationId release];
    [presentationName release];
    [diseaseId release];
    [diseaseName release];
    [dataWebView release];
    [super dealloc];
}

#pragma mark - Private Class methods
-(void)retrieveOverviewForTherapy
{
    overviewHTMLString = @"Overview Text";
}

-(NSString *)buildHTMLFromData
{
    NSMutableString *bodyHTML = [[[NSMutableString alloc] initWithString:@"<table border=\"1\" cellspacing=\"0\" cellpadding=\"5\">"] autorelease];
    if(diseaseId != nil && [diseaseId length] > 0)
    {
        // TODO: get disease record to get overview information and display it first
        
    }
    
    // use presentationId to get pathogengroups
    NSArray *pathogenGroupArray = [BDPathogenGroup retrieveAllWithParentUUID:self.presentationId];
    
    for(BDPathogenGroup *pGroup in pathogenGroupArray) {
        [bodyHTML appendFormat:@"%@",[self buildPathogenGroupHTML:pGroup]];
        
        // use pathogenGroup uuid to get pathogens
        NSArray *therapyGroupArray = [BDPathogen retrieveAllWithParentUUID:pGroup.uuid];

        for (BDTherapyGroup *tGroup in therapyGroupArray)
        {
            [bodyHTML appendFormat:@"<tr><th>%@</th></tr>",tGroup.name];
            [bodyHTML appendFormat:@"%@",[self buildTherapyGroupHTML:tGroup]];
        
            // use therapygroupid to get therapies
            NSArray *therapyArray = [BDTherapy retrieveAllWithParentUUID:tGroup.uuid];
            [bodyHTML appendString:@"<tr><th>Therapy</th><th>Dosage</th><th>Duration</th></tr>"];
            for (BDTherapy *therapy in therapyArray)
            {
                [bodyHTML appendFormat:@"%@", [self buildTherapyHTML:therapy]];
            }
         }
     }
    [bodyHTML appendString:@"</table>"];
    return [NSString stringWithString:bodyHTML];
}

-(void)loadHTMLIntoWebView 
{
    [self.dataWebView loadHTMLString:[NSString stringWithFormat:@"<html><body><font face='Helvetica' size='3.0'>%@%@<br></font></body></html>",self.overviewHTMLString, self.detailHTMLString] baseURL:[NSURL URLWithString:@""]];
    [self.dataWebView setBackgroundColor:[UIColor clearColor]];
    [self.dataWebView setOpaque:NO];
}

-(NSString *)buildPathogenGroupHTML:(BDPathogenGroup *)thePathogenGroup
{
    NSMutableString *pathogenGroupHTML = [[NSMutableString alloc] initWithCapacity:0];
    NSArray *pathogenArray = [BDPathogen retrieveAllWithParentUUID:[thePathogenGroup uuid]];

    if([pathogenArray count] > 0)
        [pathogenGroupHTML appendString:@"<tr><th>Pathogens</th></tr><tr>"];

    for(BDPathogen *pathogen in pathogenArray)
    {
       [pathogenGroupHTML appendFormat:@"<td>%@</td>",pathogen.name ];
    }
    [pathogenGroupHTML appendString:@"</tr>"];
    
    NSString *returnString = [NSString stringWithString:pathogenGroupHTML];
    [pathogenGroupHTML release];
    return returnString;
}

-(NSString *)buildTherapyGroupHTML:(BDTherapyGroup *)theTherapyGroup
{
    NSMutableString *therapyGroupHTML = [[NSMutableString alloc] initWithCapacity:0];
    
    [therapyGroupHTML appendFormat:@"<tr><th>%@</th></tr>",theTherapyGroup.name];

    NSString *returnString = [NSString stringWithString:therapyGroupHTML];
    [therapyGroupHTML release];
    return returnString;
}

-(NSString *)buildTherapyHTML:(BDTherapy *)theTherapy
{
    NSMutableString *therapyHTML = [[NSMutableString alloc] initWithCapacity:0];
    
    [therapyHTML appendFormat:@"<tr><td>%@</td>",theTherapy.name];
    
    if([theTherapy.dosage length] > 0)
        [therapyHTML appendFormat:@"<td>%@</td>",theTherapy.dosage];
    if([theTherapy.duration length] > 0)
        [therapyHTML appendFormat:@"<td>%@</td>",theTherapy.duration];
    
    
    [therapyHTML appendString:@"</tr>"];

    NSString *returnString = [NSString stringWithString:therapyHTML];
    [therapyHTML release];
    return returnString;
}

@end
