//
//  CategoryView.m
//  BDViewer
//
//  Created by Liz Dovey on 11-10-19.
//  Copyright (c) 2011 875953 Alberta, Inc. All rights reserved.
//

#import "CategoryView.h"
#import "BDCategory.h"
#import "DiseaseView.h"

@implementation CategoryView
@synthesize dataTableView;
@synthesize categoryArray;
@synthesize parentId;

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
    self.title = @"Categories";
}

-(void)viewWillAppear:(BOOL)animated
{
    [super viewWillAppear:animated];
    categoryArray = [NSArray arrayWithArray:[BDCategory retrieveAllWithParentUUID:parentId]];
}

- (void)viewDidUnload
{
    [dataTableView release];
    dataTableView = nil;
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
    [categoryArray release];
    [dataTableView release];
    [parentId release];
    [super dealloc];
}

#pragma mark - TableView DataSource

- (int)numberOfSectionsInTableView:(UITableView *)tableView {
    return 1;
}

- (int)tableView:(UITableView *)tableView numberOfRowsInSection:(NSInteger)section {
    return [categoryArray count];
}

#pragma mark - TableView Delegate

- (UITableViewCell *)tableView:(UITableView *)tableView cellForRowAtIndexPath:(NSIndexPath *)indexPath
{
    static NSString *CellIdentifier = @"Cell";
    
    UITableViewCell *cell = [dataTableView dequeueReusableCellWithIdentifier:CellIdentifier];
    if (cell == nil) {
        cell = [[[UITableViewCell alloc] initWithStyle:UITableViewCellStyleDefault reuseIdentifier:CellIdentifier] autorelease];
    }
    cell.textLabel.text = [[categoryArray objectAtIndex:indexPath.row] name ];
    // Show the disclosure indicator if the section has categories
    return cell;
}

- (void)tableView:(UITableView *)tableView didSelectRowAtIndexPath:(NSIndexPath *)indexPath
{
     DiseaseView *vwDisease = [[DiseaseView alloc] initWithNibName:@"DiseaseView" bundle:nil];
    vwDisease.parentId = [[categoryArray objectAtIndex:indexPath.row] uuid];
     [self.navigationController pushViewController:vwDisease animated:YES];
     [vwDisease release];
}

@end
