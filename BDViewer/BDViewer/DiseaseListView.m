//
//  DiseaseListView.m
//  BDViewer
//
//  Created by Liz Dovey on 11-10-19.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "DiseaseListView.h"
#import "BDDisease.h"
#import "PresentationListView.h"
#import "BDPresentation.h"
#import "DetailView.h"
#import "BDPresentation.h"
#import "BDTherapyGroup.h"

@implementation DiseaseListView
@synthesize dataTableView;
@synthesize diseaseArray;
@synthesize parentId;
@synthesize parentName;

-(id)initWithParentId:(NSString *)pParentId withParentName:(NSString *)pParentName
{
    self = [super initWithNibName:@"DiseaseListView" bundle:nil];
    if(self)
    {
        parentId = [pParentId retain];
        parentName = [pParentName retain];
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
    self.title = parentName;
}

-(void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    self.diseaseArray = [NSArray arrayWithArray:[BDDisease retrieveAllWithParentUUID:parentId]];
}

- (void)viewDidUnload
{
    [self setDataTableView:nil];
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
    [dataTableView release];
    [diseaseArray release];
    [parentId release];
    [parentName release];
    [super dealloc];
}
#pragma mark - TableView DataSource

- (int)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (int)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return [self.diseaseArray count];
}

#pragma mark - TableView Delegate

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString *CellIdentifier = @"Cell";
    
    UITableViewCell *cell = [dataTableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (cell == nil) {
        cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier] autorelease];
    }
    cell.textLabel.text = [[self.diseaseArray objectAtIndex:indexPath.row] name ];
    
    int childCount = [[BDPresentation retrieveCountWithParentUUID:[[self.diseaseArray objectAtIndex:indexPath.row] uuid]] intValue];
    cell.accessoryType = (childCount > 0) ? UITableViewCellAccessoryDisclosureIndicator : UITableViewCellAccessoryNone;
    cell.selectionStyle = (childCount > 0) ? UITableViewCellSelectionStyleBlue : UITableViewCellSelectionStyleNone;
    
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
    BDDisease *disease = [diseaseArray objectAtIndex:indexPath.row];
    int childCount = [[BDPresentation retrieveCountWithParentUUID:[[self.diseaseArray objectAtIndex:indexPath.row] uuid]] intValue];
    if(childCount > 0)
    {
        NSNumber *presentationCount = [BDPresentation retrieveCountWithParentUUID:[[self.diseaseArray objectAtIndex:indexPath.row] uuid]];
        if([presentationCount intValue] > 1)
        {
            PresentationListView *vwPresentation = [[PresentationListView alloc] initWithParentId:[disease uuid] withParentName: [disease name]];
            [self.navigationController pushViewController:vwPresentation animated:YES];
            [vwPresentation release];
        } else {
            DetailView *vwTherapy = [[DetailView alloc] initWithDiseaseId:[disease uuid] withDiseaseName: [disease name]];
            [self.navigationController pushViewController:vwTherapy animated:YES];
            [vwTherapy release];
        }
    }
}

@end
